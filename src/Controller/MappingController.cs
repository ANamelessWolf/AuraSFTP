using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nameless.Libraries.Aura.Model;
using Nameless.Libraries.Aura.Utils;
using static Nameless.Libraries.Aura.Resources.Message;
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
        /// Gets the help documentation pointers
        /// </summary>
        public override HelpPointer[] Help => new HelpPointer[] {
            new HelpPointer (this, String.Empty, 144, 176),
            new HelpPointer (this, "dir", 152, 159),
            new HelpPointer (this, "file", 161, 168),
            new HelpPointer (this, "remove", 170, 176)
        };
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
                            this.MapDirectoryAbsolutePath (prj, this.Args[0], this.Args[1]);
                        else if (this.Args.Length == 1)
                            this.MapDirectoryRelativePath (prj, this.Args[0]);
                        else
                            throw new Exception (this.GetErrorArgsMessage (this.Option));
                        break;
                    case "file":
                        if (this.Args.Length == 2)
                            this.MapFileAbsolutePath (prj, this.Args[0], this.Args[1]);
                        else if (this.Args.Length == 1)
                            this.MapFileRelativePath (prj, this.Args[0]);
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
        private void RemovePath (Project prj, string localPath) {
            String pth = Path.Combine (prj.Data.ProjectCopy, localPath).ToLower ();
            //First check in files
            MappedPath[] fileData = this.RemovePath (prj.Data.Map.Files, pth),
                dirData = this.RemovePath (prj.Data.Map.Directories, pth);
            if (fileData != null || dirData != null) {
                prj.SaveProject (this.ConfigFile);
                Console.WriteLine (MSG_INF_MAP_REMOVED, pth);
            } else
                Console.WriteLine (MSG_ERR_MAP_REM_PTH, pth);
        }
        /// <summary>
        /// Remove the a path from a collection of paths
        /// </summary>
        /// <param name="paths">The collection of paths</param>
        /// <param name="projectPath">The project path to remove</param>
        /// <returns>The new path collection if no path is removed this returns null</returns>
        private MappedPath[] RemovePath (MappedPath[] paths, String projectPath) {
            int index = -1;
            MappedPath[] data = null;
            index = Array.FindIndex (paths, x => x.ProjectCopy.ToLower () == projectPath);
            if (index > 0)
                data = paths.Where (x => x.ProjectCopy.ToLower () != projectPath).ToArray ();
            return data;
        }
        /// <summary>
        /// Maps a file relative to the project file,
        /// using as base path the project path
        /// </summary>
        /// <param name="prj">The active project</param>
        /// <param name="localPath">The relative path using simple slash as separator</param>
        private void MapFileRelativePath (Project prj, string pth) {
            RelativeMappedPath path = MappingUtils.GetMappedPath (prj, pth);
            this.MapFile (prj, path);
        }
        /// <summary>
        /// Maps a file absolute to the project file,
        /// using as base path the project path
        /// </summary>
        /// <param name="prj">The active project</param>
        /// <param name="localPath">The relative path using simple slash as separator</param>
        private void MapFileAbsolutePath (Project prj, string localFilePath, string remoteFilePath) {
            MappedPath path = MappingUtils.GetMappedPath (prj, localFilePath, remoteFilePath);
            this.MapFile (prj, path);
        }
        /// <summary>
        /// Maps a file to the project file
        /// </summary>
        /// <param name="prj">The active project</param>
        /// <param name="path">The mapped file data</param>
        private void MapFile (Project prj, MappedPath path) {
            String error_msg = null;
            var result = AuraSftpClient.SFTPTransactionGen<MappedPath> (prj.Connection.Data,
                (RenCiSftpClient client) => {
                    String rPth = path.GetFullRemotePath ();
                    if (client.Exists (rPth)) {
                        var entry = client.Get (rPth);
                        path.RemoteVersion = entry.LastAccessTime;
                        Boolean mapExist = prj.Data.Map.Directories.FirstOrDefault (x => x.GetFullRemotePath () == entry.FullName) != null;
                        if (!entry.IsDirectory && !mapExist) {
                            if (!Directory.Exists (new FileInfo (path.GetFullProjectCopy ()).Directory.FullName))
                                Directory.CreateDirectory (new FileInfo (path.GetFullProjectCopy ()).Directory.FullName);
                            if (!Directory.Exists (new FileInfo (path.GetFullServerCopy ()).Directory.FullName))
                                Directory.CreateDirectory (new FileInfo (path.GetFullServerCopy ()).Directory.FullName);
                        } else if (entry.IsDirectory)
                            error_msg = String.Format (MSG_ERR_MAP_REM_PTH_NOT_FILE, rPth, HelpCommand, "file");
                        else if (mapExist)
                            error_msg = String.Format (MSG_ERR_MAP_AlREADY_MAPPED, rPth);
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
        /// Maps a directory relative to the project file,
        /// using as base path the project path
        /// </summary>
        /// <param name="prj">The active project</param>
        /// <param name="localPath">The relative path using simple slash as separator</param>
        private void MapDirectoryRelativePath (Project prj, string pth) {
            RelativeMappedPath path = MappingUtils.GetMappedPath (prj, pth);
            this.MapDirectory (prj, path);
        }
        /// <summary>
        /// Maps a directory to the project file
        /// </summary>
        /// <param name="prj">The active project</param>
        /// <param name="localPath">The local path</param>
        /// <param name="remotePath">The remote path</param>
        private void MapDirectoryAbsolutePath (Project prj, string localPath, string remotePath) {
            String rPth = prj.Connection.Data.RootDir + remotePath;
            MappedPath path = MappingUtils.GetMappedPath (prj, localPath, remotePath);
            this.MapDirectory (prj, path);
        }
        /// <summary>
        /// Maps a directory to the project file
        /// </summary>
        /// <param name="prj">The project file</param>
        /// <param name="path">The mapped directory data</param>
        private void MapDirectory (Project prj, MappedPath path) {
            String error_msg = null;
            var result = AuraSftpClient.SFTPTransactionGen<MappedPath> (prj.Connection.Data,
                (RenCiSftpClient client) => {
                    String rPth = path.GetFullRemotePath ();
                    if (client.Exists (rPth)) {
                        var entry = client.Get (rPth);
                        path.RemoteVersion = entry.LastAccessTime;
                        Boolean mapExist = prj.Data.Map.Directories.FirstOrDefault (x => x.GetFullRemotePath () == entry.FullName) != null;
                        if (entry.IsDirectory && !mapExist) {
                            if (!Directory.Exists (path.GetFullProjectCopy ()))
                                Directory.CreateDirectory (path.GetFullProjectCopy ());
                            if (!Directory.Exists (path.GetFullServerCopy ()))
                                Directory.CreateDirectory (path.GetFullServerCopy ());
                        } else if (!entry.IsDirectory)
                            error_msg = String.Format (MSG_ERR_MAP_REM_PTH_NOT_DIR, rPth, HelpCommand, "dir");
                        else if (mapExist)
                            error_msg = String.Format (MSG_ERR_MAP_AlREADY_MAPPED, rPth);
                    } else
                        error_msg = String.Format (MSG_ERR_MAP_REM_PTH, rPth);
                    return path;
                });
            if (result != null && error_msg == null) {
                prj.Data.Map.Directories = prj.Data.Map.Directories.Union (new MappedPath[] { result }).ToArray ();
                prj.SaveProject (this.ConfigFile);
                Console.WriteLine (String.Format (MSG_INF_MAP_CREATED, result.GetFullRemotePath (), result.GetFullProjectCopy ()));
            } else if (error_msg != null)
                Console.WriteLine (error_msg);
        }
    }
}