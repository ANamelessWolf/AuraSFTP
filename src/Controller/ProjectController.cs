using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Nameless.Libraries.Aura.Resources.Message;
using Nameless.Libraries.Aura.Model;
using Nameless.Libraries.Aura.Utils;
using RenciSftpClient = Renci.SshNet.SftpClient;
using Nameless.Libraries.Aura.Utils.DiffMatchPatch;

namespace Nameless.Libraries.Aura.Controller {
    /// <summary>
    /// This class controls the project functionality
    /// </summary>
    public class ProjectController : CommandController {
        /// <summary>
        /// Extended options
        /// </summary>
        const String EXT_OPT_REPLACE = "replace";
        /// <summary>
        /// Gets the help command name
        /// </summary>
        public override string HelpCommand => "project";
        /// <summary>
        /// The command valid options
        /// </summary>
        protected override string[] ValidOptions => new String[] { "new", "add", "pull", "push", "check", "site" };
        /// <summary>
        /// Command shortcut
        /// </summary>
        public override string CommandShortcut => "-p";
        /// <summary>
        /// Initialize a new instance for a command controller
        /// </summary>
        /// <param name="args">The command arguments</param>
        public ProjectController (string[] args) : base (args) { }
        /// <summary>
        /// Runs the given command
        /// <throw>An exception is thrown when the given option is invalid</throw>
        /// </summary>
        public override void RunCommand () {
            if (this.ValidOptions.Contains (this.Option))
                switch (this.Option) {
                    case "new":
                        if (this.Args.Length == 2)
                            this.CreateProject (this.Args[0], this.Args[1]);
                        else
                            throw new Exception (this.GetErrorArgsMessage (this.Option));
                        break;
                    case "add":
                        if (this.Args.Length == 2)
                            this.AddToServer (ProjectUtils.OpenProject (), this.Args[0], this.Args[1]);
                        else
                            throw new Exception (this.GetErrorArgsMessage (this.Option));
                        break;
                    case "pull":
                        try {
                            Boolean replace = this.Args.Length > 0 ? this.Args[0] == EXT_OPT_REPLACE : false;
                            if (this.Args.Length > 0 && this.Args[0] != EXT_OPT_REPLACE)
                                throw new Exception (String.Format (MSG_ERR_BAD_OPTION, this.CommandShortcut, this.HelpCommand + " replace"));
                            this.PullFromServer (ProjectUtils.OpenProject (), replace);
                        } catch (System.Exception exc) {
                            throw exc;
                        }
                        break;
                    case "push":
                        try {
                            this.PushToServer (ProjectUtils.OpenProject ());
                        } catch (System.Exception exc) {
                            throw exc;
                        }
                        break;
                    case "site":
                        try {
                            if (this.Args.Length == 1) {
                                String siteName = this.Args[0];
                                this.SetSite (ProjectUtils.OpenProject (), siteName);
                            } else
                                throw new Exception (String.Format (MSG_ERR_BAD_OPTION, this.CommandShortcut, this.HelpCommand));
                        } catch (System.Exception exc) {
                            throw exc;
                        }
                        break;
                    case "check":
                        try {
                            this.CheckFiles (ProjectUtils.OpenProject ());
                        } catch (System.Exception exc) {
                            throw exc;
                        }
                        break;
                }
            else
                throw new Exception (String.Format (MSG_ERR_BAD_OPTION, this.CommandShortcut, this.HelpCommand));
        }
        /// <summary>
        /// Adds a file or a directory to the server
        /// </summary>
        /// <param name="prj">The current project</param>
        /// <param name="localPath">The local path</param>
        /// <param name="remotePath">The remote path</param>
        private void AddToServer (Project prj, string localPath, string remotePath) {
            String prjPath = Environment.CurrentDirectory;
            String configFile = Path.Combine (prjPath, ".ssh", "config.json");
            if (File.Exists (localPath)) //is a file
            {
                var mapped = prj.Data.Map.Files.FirstOrDefault (x => x.ProjectCopy == localPath);
                if (mapped == null) {
                    File.Copy (localPath, localPath.Replace (prj.Data.ProjectCopy, prj.Data.ServerCopy));
                    MappedPath newMap = new MappedPath () {
                        ProjectCopy = localPath,
                        ServerCopy = localPath.Replace (prj.Data.ProjectCopy, prj.Data.ServerCopy),
                        RemotePath = remotePath,
                        RemoteVersion = DateTime.Now,
                        LocaVersion = DateTime.Now
                    };
                    SftpUtils.UploadFiles (new MappedPath[] { newMap }, prj);
                    prj.Data.Map.Files = prj.Data.Map.Files.Union (new MappedPath[] { newMap }).ToArray ();
                    prj.SaveProject (configFile);
                    Console.WriteLine (String.Format (MSG_INF_MAP_CREATED, localPath, remotePath));
                } else
                    throw new Exception (String.Format (MSG_ERR_MAP_AlREADY_MAPPED, localPath));
            } else if (Directory.Exists (localPath)) {
                var mapped = prj.Data.Map.Files.FirstOrDefault (x => x.ProjectCopy == localPath);
                if (mapped == null) {
                    var files = prj.Filter.FilesInDirectory (localPath);
                    foreach (String file in files)
                        File.Copy (file, file.Replace (prj.Data.ProjectCopy, prj.Data.ServerCopy));
                    SftpUtils.UploadFiles (files.Select (x =>
                        new MappedPath () {
                            ProjectCopy = x,
                                ServerCopy = x.Replace (prj.Data.ProjectCopy, prj.Data.ServerCopy),
                                RemotePath = remotePath + "/" + x.Substring (x.LastIndexOf ('\\')),
                                RemoteVersion = DateTime.Now,
                                LocaVersion = DateTime.Now
                        }
                    ), prj);
                    MappedPath newMap = new MappedPath () {
                        ProjectCopy = localPath,
                        ServerCopy = localPath.Replace (prj.Data.ProjectCopy, prj.Data.ServerCopy),
                        RemotePath = remotePath,
                        RemoteVersion = DateTime.Now,
                        LocaVersion = DateTime.Now
                    };
                    prj.Data.Map.Directories = prj.Data.Map.Directories.Union (new MappedPath[] { newMap }).ToArray ();
                    prj.SaveProject (configFile);
                    Console.WriteLine (String.Format (MSG_INF_MAP_CREATED, localPath, remotePath));
                }
            } else
                throw new Exception (MSG_ERR_PRJ_BAD_LOC_PTH);
        }
        /// <summary>
        /// Push the files to the server only the mapped fies are push to the server
        /// </summary>
        /// <param name="prj">The current project</param>
        private void PushToServer (Project prj) {
            if (prj.Data.Map.Files.Count () > 0 || prj.Data.Map.Directories.Count () > 0) {
                List<MappedPath> files = new List<MappedPath> ();
                foreach (var file in prj.Data.Map.Files)
                    if (files.Count (x => x.ProjectCopy == file.ProjectCopy) == 0)
                        files.Add (file);
                foreach (var dir in prj.Data.Map.Directories)
                    this.GetPaths (ref files, new DirectoryInfo (dir.ProjectCopy), prj, dir);
                diff_match_patch dmp = new diff_match_patch ();
                //Only uploads files with diff
                String[] cExt = Program.Settings.ComparableFilesExt;
                Boolean isComparable;
                var filesToUpload = files.Where (x => {
                    isComparable = x.ProjectCopy.IsComparable (cExt);
                    return !isComparable || (isComparable && !dmp.AreFilesEquals (x.ProjectCopy, x.ServerCopy));
                });
                if (filesToUpload.Count () > 0)
                    SftpUtils.UploadFiles (filesToUpload, prj);
                else
                    Console.WriteLine (MSG_INF_PRJ_PUSH_NO_CHANGES);
            } else
                throw new Exception (MSG_ERR_PRJ_PULL_EMPTY_MAP);
        }
        /// <summary>
        /// Check the files from the current cached and list the files that has 
        /// differences
        /// </summary>
        /// <param name="prj">The current project</param>
        private void CheckFiles (Project prj) {
            if (prj.Data.Map.Files.Count () > 0 || prj.Data.Map.Directories.Count () > 0) {
                this.PullFromServer (prj, false);
                List<MappedPath> files = new List<MappedPath> ();
                foreach (var file in prj.Data.Map.Files)
                    if (files.Count (x => x.ProjectCopy == file.ProjectCopy) == 0)
                        files.Add (file);
                foreach (var dir in prj.Data.Map.Directories)
                    this.GetPaths (ref files, new DirectoryInfo (dir.ProjectCopy), prj, dir);
                diff_match_patch dmp = new diff_match_patch ();
                //Only uploads files with diff
                String[] cExt = Program.Settings.ComparableFilesExt;
                Boolean isComparable;
                var filesToUpload = files.Where (x => {
                    isComparable = x.ProjectCopy.IsComparable (cExt);
                    return !isComparable || (isComparable && !dmp.AreFilesEquals (x.ProjectCopy, x.ServerCopy));
                });
                if (filesToUpload.Count () > 0) {
                    Console.WriteLine (MSG_INF_FILES_WITH_CHANGES);
                    filesToUpload.ToList ().ForEach (x =>
                        Console.WriteLine (x.ProjectCopy));
                } else
                    Console.WriteLine (MSG_INF_PRJ_NO_CHANGES);
            } else
                Console.WriteLine (MSG_INF_PRJ_NO_CHANGES);
        }
        /// <summary>
        /// Gets the mapped file paths from the given directory, using the project filtering
        /// The result is saved in the files List 
        /// </summary>
        /// <param name="files">The list of mapped files</param>
        /// <param name="prj">The current project</param>
        /// <param name="dir">The mapped path directory</param>
        private void GetPaths (ref List<MappedPath> files, DirectoryInfo dirInfo, Project prj, MappedPath dirMap) {
            String servercopy = prj.Data.ServerCopy,
                fileInServerCopy;
            foreach (var file in dirInfo.GetFiles ()) {
                //Files must exist in server copy and shouldn't be already added
                fileInServerCopy = file.FullName.Replace (prj.Data.ProjectCopy, prj.Data.ServerCopy);
                if (File.Exists (fileInServerCopy) && files.Count (x => file.FullName == x.ProjectCopy) == 0)
                    files.Add (SftpUtils.GetMappedPath (file, dirMap.ProjectCopy, dirMap.RemotePath, dirMap.ServerCopy));
            }
            foreach (var dir in dirInfo.GetDirectories ())
                GetPaths (ref files, dir, prj, dirMap);
        }

        /// <summary>
        /// Pull the files from the server that are defined on the project
        /// configuration file Map
        /// </summary>
        /// <param name="prj">The current project</param>
        /// <param name="replace">True if the pulled files will remove the local ones</param>
        private void PullFromServer (Project prj, Boolean replace) {
            if (prj.Data.Map.Files.Count () > 0 || prj.Data.Map.Directories.Count () > 0) {
                AuraSftpClient.SFTPTransactionVoid (prj.Connection.Data, (RenciSftpClient client) => {
                    var dirs = prj.Data.Map.Directories;
                    var files = prj.Data.Map.Files;
                    SftpFilter filter = prj.Filter;
                    foreach (var dir in dirs)
                        client.Download (prj.Connection.Data, dir, filter, replace);
                    foreach (var file in files)
                        prj.Connection.Data.Download (file, replace);
                });
            } else
                throw new Exception (MSG_ERR_PRJ_PULL_EMPTY_MAP);
        }

        /// <summary>
        /// Set the project connection site configuration data
        /// </summary>
        /// <param name="prj">The current project</param>
        /// <param name="siteName">The site name</param>
        private void SetSite (Project prj, String siteName) {
            var site = Program.Settings.Sites.FirstOrDefault (x => x.Site.ToLower () == siteName.ToLower ());
            if (site == null)
                throw new Exception (String.Format (MSG_ERR_SITE_NOT_FOUND, siteName));
            else {
                prj.Connection = site;
                String localPath = Environment.CurrentDirectory;
                String configFile = Path.Combine (localPath, ".ssh", "config.json");
                prj.SaveProject (configFile);
            }
        }

        /// <summary>
        /// This method creates a new project in the given path
        /// </summary>
        /// <param name="projectName">The name of the project</param>
        /// <param name="projectPath">The path of the project</param>
        private void CreateProject (string projectName, string projectPath) {
            Boolean pathExists = Directory.Exists (projectPath),
                prjDirExists = pathExists?Directory.Exists (Path.Combine (projectPath, projectName)) : false,
                prjIsEmpty = prjDirExists?Directory.GetDirectories (projectPath).Length + Directory.GetFiles (projectPath).Length == 0 : true;
            if (pathExists) //&& prjIsEmpty)
                ProjectUtils.InitProject (projectName, projectPath);
            else if (!pathExists)
                throw new Exception (MSG_ERR_NEW_PRJ_MISS_DIR);
            Console.WriteLine (MSG_INF_NEW_PRJ, projectName, projectPath);
        }

    }
}