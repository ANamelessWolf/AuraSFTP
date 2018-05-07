using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Nameless.Libraries.Aura.data.Message;
using Nameless.Libraries.Aura.Model;
using Nameless.Libraries.Aura.Utils;
using RenciSftpClient = Renci.SshNet.SftpClient;
namespace Nameless.Libraries.Aura.Controller {
    /// <summary>
    /// Initialize a new instance of the project controller
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
        protected override string[] ValidOptions => new String[] { "new", "add", "pull", "push" };
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
                }
            else
                throw new Exception (String.Format (MSG_ERR_BAD_OPTION, this.CommandShortcut, this.HelpCommand));
        }
        /// <summary>
        /// Push the files to the server that are defined on the project
        /// configuration file Map
        /// </summary>
        /// <param name="prj">The current project</param>
        private void PushToServer (Project project) {
            throw new NotImplementedException ();
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
            Console.WriteLine (MSG_INF_NEW_PRJ);
        }

    }
}