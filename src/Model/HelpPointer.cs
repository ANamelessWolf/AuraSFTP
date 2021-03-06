using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nameless.Libraries.Aura.Controller;

namespace Nameless.Libraries.Aura.Model {

    public class HelpPointer {
        /// <summary>
        /// The name of the command to ask for help
        /// </summary>
        public String Command;
        /// <summary>
        /// The name of the option to ask for help
        /// </summary>
        public String Option;
        /// <summary>
        /// The help document starting line
        /// </summary>
        public int StartLine;
        /// <summary>
        /// The help document ending line
        /// </summary>
        public int EndLine;
        /// <summary>
        /// The help content
        /// </summary>
        private String Content;
        /// <summary>
        /// Initialize a new instance of a help controller
        /// </summary>
        /// <param name="controller">The command controller</param>
        /// <param name="start">The index where the help lines starts. Starting index at 1</param>
        /// <param name="end">The index where the help lines end</param>
        public HelpPointer (CommandController controller, String option, int start, int end) {
            this.Command = controller.HelpCommand;
            this.Option = option;
            this.StartLine = start - 1;
            this.EndLine = end - 1;
        }
        /// <summary>
        /// Initialize a new instance of a help controller
        /// </summary>
        /// <param name="start">The index where the help lines starts. Starting index at 1</param>
        /// <param name="end">The index where the help lines end</param>
        public HelpPointer (String option, int start, int end) {
            this.Command = "";
            this.Option = option;
            this.StartLine = start - 1;
            this.EndLine = end - 1;
        }
        /// <summary>
        /// Sets the help content
        /// </summary>
        /// <param name="lines">The help file lines</param>
        public void SetHelpContent (String[] lines) {
            this.Content = String.Join ("\n", lines.Skip (this.StartLine).Take ((this.EndLine - this.StartLine) + 1));
        }
        /// <summary>
        /// Gets the help content
        /// </summary>
        public String GetHelp () {
            return this.Content;
        }
    }
}