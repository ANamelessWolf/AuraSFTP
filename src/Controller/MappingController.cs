using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Nameless.Libraries.Aura.data.Message;
namespace Nameless.Libraries.Aura.Controller {

    public class MappingController : CommandController {
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
        protected override string[] ValidOptions => new String[] { "dir", "file" };
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
            if (this.ValidOptions.Contains (this.Option))
                switch (this.Option) {
                    case "dir":
                        if (this.Args.Length == 2)
                            this.MapDirectory (this.Args[0], this.Args[1]);
                        else
                            throw new Exception (this.GetErrorArgsMessage (this.Option));
                        break;
                    case "file":
                        if (this.Args.Length == 2)
                            this.MapFile (this.Args[0], this.Args[1]);
                        else
                            throw new Exception (this.GetErrorArgsMessage (this.Option));
                        break;
                }
            else
                throw new Exception (String.Format (MSG_ERR_BAD_OPTION, this.CommandShortcut, this.HelpCommand));

        }

        private void MapFile (string localFilePath, string remoteFilePath) {
            throw new NotImplementedException ();
        }

        private void MapDirectory (string localPath, string remotePath) {
            throw new NotImplementedException ();
        }
    }
}