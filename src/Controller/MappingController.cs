using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nameless.Libraries.Aura.Model;
using Nameless.Libraries.Aura.Utils;
using static Nameless.Libraries.Aura.Resources .Message;
using RenCiSftpClient = Renci.SshNet.SftpClient;
namespace Nameless.Libraries.Aura.Controller {

    public class MappingController : CommandController {
        /// <summary>
        /// The path to the configuration file
        /// </summary>
        public String ConfigFile;
        /// <summary>
        /// Command shortcut
        /// </summary>
        public override string CommandShortcut => "-m";
        /// <summary>
        /// Gets the help command name
        /// </summary>
        public override string HelpCommand => "mapping";
        /// <summary>
        /// The command valid options
        /// </summary>
        protected override string[] ValidOptions => new String[] { "dir", "file", "remove" };
        /// <summary>
        /// Initialize a new instance for a command controller
        /// </summary>
        /// <param name="args">The command arguments</param>
        public MappingController (string[] args) : base (args) { }
        /// <summary>
        /// Runs the given command
        /// <throw>An exception is thrown when the given option is invalid</throw>
        /// </summary>
        public override void RunCommand () {
            String localPath = Environment.CurrentDirectory;
            this.ConfigFile = Path.Combine (localPath, ".ssh", "config.json");
            Boolean optionsAreValid = this.ValidOptions.Contains (this.Option),
                sshProjectExists = File.Exists (this.ConfigFile);
            if (optionsAreValid && sshProjectExists) {
                Project prj = this.ConfigFile.OpenProjectFile ();
                switch (this.Option) {
                    case "dir":
                        if (this.Args.Length == 2)
                            this.MapDirectory (prj, this.Args[0], this.Args[1]);
                        else
                            throw new Exception (this.GetErrorArgsMessage (this.Option));
                        break;
                    case "file":
                        if (this.Args.Length == 2)
                            this.MapFile (prj, this.Args[0], this.Args[1]);
                        else
                            throw new Exception (this.GetErrorArgsMessage (this.Option));
                        break;
                    case "remove":
                        if (this.Args.Length == 1)
                            this.RemovePath (prj, this.Args[0]);
                        else
                            throw new Exception (this.GetErrorArgsMessage (this.Option));
                        break;
                }
            } else if (!sshProjectExists)
                throw new Exception (MSG_ERR_PRJ_MISS);
            else if (!optionsAreValid)
                throw new Exception (String.Format (MSG_ERR_BAD_OPTION, this.CommandShortcut, this.HelpCommand));

        }
        /// <summary>
        /// Removes a path from the project file
        /// </summary>
        /// <param name="prj">The active project</param>
        /// <param name="localPath">The local file path</param>
        private void RemovePath (Project prj, string v) {
            throw new NotImplementedException ();
        }

        /// <summary>
        /// Maps a file to the project file
        /// </summary>
        /// <param name="prj">The active project</param>
        /// <param name="localFilePath">The local file path</param>
        /// <param name="remoteFilePath">The remote file path</param>
        private void MapFile (Project prj, string localFilePath, string remoteFilePath) {
            String error_msg = null;
            var result = AuraSftpClient.SFTPTransactionGen<MappedPath> (prj.Connection.Data,
                (RenCiSftpClient client) => {
                    MappedPath path = null;
                    String rPth = prj.Connection.Data.RootDir + remoteFilePath;
                    if (client.Exists (rPth)) {
                        var entry = client.Get (rPth);
                        path = prj.Data.Map.Files.FirstOrDefault (x => x.RemotePath == entry.FullName);
                        Boolean mapExist = path != null;
                        if (!entry.IsDirectory && !mapExist) {
                            path = new MappedPath () {
                                ProjectCopy = Path.Combine (prj.Data.ProjectCopy, localFilePath),
                                ServerCopy = Path.Combine (prj.Data.ServerCopy, localFilePath),
                                RemotePath = rPth,
                                RemoteVersion = entry.Attributes.LastAccessTime,
                                LocaVersion = DateTime.Now
                            };
                            if (!Directory.Exists (new FileInfo (path.ProjectCopy).Directory.FullName))
                                Directory.CreateDirectory (new FileInfo (path.ProjectCopy).Directory.FullName);
                            if (!Directory.Exists (new FileInfo (path.ServerCopy).Directory.FullName))
                                Directory.CreateDirectory (new FileInfo (path.ServerCopy).Directory.FullName);
                        } else if (entry.IsDirectory)
                            error_msg = String.Format (MSG_ERR_MAP_REM_PTH_NOT_FILE, rPth, HelpCommand, "file");
                        else if (mapExist)
                            error_msg = String.Format (MSG_ERR_MAP_AlREADY_MAPPED, rPth, HelpCommand, "remove");
                    } else
                        error_msg = String.Format (MSG_ERR_MAP_REM_PTH, rPth);
                    return path;
                });
            if (result != null && error_msg == null) {
                prj.Data.Map.Files = prj.Data.Map.Files.Union (new MappedPath[] { result }).ToArray ();
                prj.SaveProject (this.ConfigFile);
                Console.WriteLine (String.Format (MSG_INF_MAP_CREATED, result.RemotePath, result.ProjectCopy));
            } else if (error_msg != null)
                Console.WriteLine (error_msg);
        }
        /// <summary>
        /// Maps a directory to the project file
        /// </summary>
        /// <param name="prj">The active project</param>
        /// <param name="localPath">The local path</param>
        /// <param name="remotePath">The remote path</param>
        private void MapDirectory (Project prj, string localPath, string remotePath) {
            String error_msg = null;
            var result = AuraSftpClient.SFTPTransactionGen<MappedPath> (prj.Connection.Data,
                (RenCiSftpClient client) => {
                    MappedPath path = null;
                    String rPth = prj.Connection.Data.RootDir + remotePath;
                    if (client.Exists (rPth)) {
                        var entry = client.Get (rPth);
                        path = prj.Data.Map.Directories.FirstOrDefault (x => x.RemotePath == entry.FullName);
                        Boolean mapExist = path != null;
                        if (entry.IsDirectory && !mapExist) {
                            path = new MappedPath () {
                                ProjectCopy = Path.Combine (prj.Data.ProjectCopy, localPath),
                                ServerCopy = Path.Combine (prj.Data.ServerCopy, localPath),
                                RemotePath = rPth,
                                RemoteVersion = entry.Attributes.LastAccessTime,
                                LocaVersion = DateTime.Now
                            };
                            if (!Directory.Exists (path.ProjectCopy))
                                Directory.CreateDirectory (path.ProjectCopy);
                            if (!Directory.Exists (path.ServerCopy))
                                Directory.CreateDirectory (path.ServerCopy);
                        } else if (!entry.IsDirectory)
                            error_msg = String.Format (MSG_ERR_MAP_REM_PTH_NOT_DIR, rPth, HelpCommand, "dir");
                        else if (mapExist)
                            error_msg = String.Format (MSG_ERR_MAP_AlREADY_MAPPED, rPth, HelpCommand, "remove");
                    } else
                        error_msg = String.Format (MSG_ERR_MAP_REM_PTH, rPth);
                    return path;
                });
            if (result != null && error_msg == null) {
                prj.Data.Map.Directories = prj.Data.Map.Directories.Union (new MappedPath[] { result }).ToArray ();
                prj.SaveProject (this.ConfigFile);
                Console.WriteLine (String.Format (MSG_INF_MAP_CREATED, result.RemotePath, result.ProjectCopy));
            } else if (error_msg != null)
                Console.WriteLine (error_msg);
        }
    }
}