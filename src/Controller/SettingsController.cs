using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Nameless.Libraries.Aura.data.Message;
using Nameless.Libraries.Aura.Model;
using Nameless.Libraries.Aura.Utils;
using RenciSftpClient = Renci.SshNet.SftpClient;
namespace Nameless.Libraries.Aura.Controller {

    public class SettingsController : CommandController {
        /// <summary>
        /// The command shortcut
        /// </summary>
        /// <returns>The command shortcut</returns>
        public override string CommandShortcut => "-c";
        /// <summary>
        /// The help command
        /// </summary>
        public override string HelpCommand => "config";
        /// <summary>
        /// The validation options
        /// </summary>
        protected override string[] ValidOptions => new String[] { "site" };
        /// <summary>
        /// Initialize a new instance for a command controller
        /// </summary>
        /// <param name="args">The command arguments</param>
        public SettingsController (string[] args) : base (args) { }
        /// <summary>
        /// Runs the given command
        /// <throw>An exception is thrown when the given option is invalid</throw>
        /// </summary>
        public override void RunCommand () {
            if (this.ValidOptions.Contains (this.Option))
                switch (this.Option) {
                    case "site":
                        break;
                } else
                    throw new Exception (String.Format (MSG_ERR_BAD_OPTION, this.CommandShortcut, this.HelpCommand));

        }
    }
}