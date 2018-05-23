using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Nameless.Libraries.Aura.Utils.CommandUtils;
using static Nameless.Libraries.Aura.Resources.Message;
namespace Nameless.Libraries.Aura.Controller {
    /// <summary>
    /// Initialize a new instance of the project controller
    /// </summary>
    public abstract class CommandController {
        /// <summary>
        /// The command arguments
        /// </summary>
        public String[] Args;
        /// <summary>
        /// The command valid options
        /// </summary>
        protected abstract String[] ValidOptions { get; }
        /// <summary>
        /// The command selected option
        /// </summary>
        public String Option;
        /// <summary>
        /// Command shortcut
        /// </summary>
        public abstract String CommandShortcut { get; }
        /// <summary>
        /// Gets the help command name
        /// </summary>
        public abstract String HelpCommand { get; }
        /// <summary>
        /// Initialize a new instance for a command controller
        /// </summary>
        /// <param name="args">The command arguments</param>
        public CommandController (String[] args) {
            String[] opt, cmdArgs;
            if (args.Length > 0) {
                args.Split (1, out opt, out cmdArgs);
                this.Option = opt[0];
                this.Args = cmdArgs;
            } else
                throw new Exception (String.Format (MSG_ERR_BAD_CMD_OPTION, HelpCommand));
        }
        /// <summary>
        /// Runs the given command
        /// </summary>
        public abstract void RunCommand ();
        /// <summary>
        /// Gets the specific error message for the given option
        /// </summary>
        /// <param name="option">The selected option</param>
        /// <returns>The message error for the given option</returns>
        public string GetErrorArgsMessage (String option) {
            return string.Format (MSG_ERR_BAD_CMD_ARGS, HelpCommand, option);
        }
    }
}