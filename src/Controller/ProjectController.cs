using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Nameless.Libraries.Aura.data.Message;
using Nameless.Libraries.Aura.Utils;
namespace Nameless.Libraries.Aura.Controller {
    /// <summary>
    /// Initialize a new instance of the project controller
    /// </summary>
    public class ProjectController : CommandController {
        /// <summary>
        /// Gets the help command name
        /// </summary>
        public override string HelpCommand => "project";
        /// <summary>
        /// The command valid options
        /// </summary>
        protected override string[] ValidOptions => new String[] { "new", "add" };
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
                }
            else
                throw new Exception (String.Format (MSG_ERR_BAD_OPTION, this.CommandShortcut, this.HelpCommand));
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
            if (pathExists && prjIsEmpty)
                ProjectUtils.InitProject (projectName, projectPath);
            else if (!pathExists)
                throw new Exception (MSG_ERR_NEW_PRJ_MISS_DIR);
            else if (!prjIsEmpty)
                throw new Exception (MSG_ERR_PRJ_NOT_EMPTY);
                Console.WriteLine(MSG_INF_NEW_PRJ);
        }
    }
}