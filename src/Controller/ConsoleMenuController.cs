using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nameless.Libraries.Aura.Model;
using Nameless.Libraries.Aura.Utils;
using static Nameless.Libraries.Aura.Resources.Message;
using static Nameless.Libraries.Aura.Utils.CommandUtils;
namespace Nameless.Libraries.Aura.Controller {
    /// <summary>
    /// This class controls the menu controller
    /// </summary>
    public class ConsoleMenuController {
        /// <summary>
        /// Access the application current connection
        /// </summary>
        public static SiteDefinition Connection {
            get {
                int selCred = Program.Settings.SelectedSite;
                return Program.Settings.Sites[selCred];
            }
        }
        /// <summary>
        /// The current site credentials
        /// </summary>
        private SiteCredentials Credentials;
        /// <summary>
        /// Defines the console menu valid parameters
        /// </summary>
        private readonly String[] ValidParameters = new String[] { "-p", "-h", "-i", "-m", "-s", "-v" };
        /// <summary>
        /// Defines the help menu commands
        /// </summary>
        private readonly String[] HelpCommands = new String[] { "project", "ignore", "mapping", "site" };
        /// <summary>
        /// Current Site
        /// </summary>
        public String SiteName { get => Program.Settings.Sites[Program.Settings.SelectedSite].Site; }
        /// <summary>
        /// Initialize a new instance of the user settings
        /// </summary>
        /// <param name="sett">The application user settings</param>
        public ConsoleMenuController (UserSettings sett) {
            int selCred = sett.SelectedSite;
            if (selCred < sett.Sites.Length)
                this.Credentials = sett.Sites[selCred].Data;
            else if (sett.Sites.Length > 0) {
                sett.SelectedSite = 0;
                this.Credentials = sett.Sites[0].Data;
                sett.Save ();
            }
        }

        /// <summary>
        /// Gets the console menu
        /// </summary>
        /// <returns>The console menu</returns>
        public string GetMenu () {
            String helpFile = String.Format ("help.{0}.txt", Program.Settings.Language);
            String bin = SiteUtils.GetBinPath ();
            helpFile = Path.Combine (bin, "data", helpFile);
            String[] menu = File.ReadAllLines (helpFile);
            return String.Join ("\n", menu);
        }
        /// <summary>
        /// Excecutes the given command parameter
        /// </summary>
        /// <param name="args">The arguments used to run the command</param>
        public void RunCommand (string[] args) {
            if (args.Length > 0 && this.ValidParameters.Contains (args[0])) {
                CommandController cmd = null;
                String[] opt, cmdArgs;
                args.Split (1, out opt, out cmdArgs);
                switch (opt[0]) {
                    case "-p":
                        cmd = new ProjectController (cmdArgs);
                        break;
                    case "-m":
                        cmd = new MappingController (cmdArgs);
                        break;
                    case "-s":
                        cmd = new SiteController (cmdArgs);
                        break;
                    case "-i":
                        cmd = new IgnoreController (cmdArgs);
                        break;
                    case "-h":
                        cmdArgs = cmdArgs.Length > 0 ? cmdArgs : new String[] { "" };
                        HelpPointer help = this.GetHelpCommand (cmdArgs);
                        Console.WriteLine (help.GetHelp ());
                        break;
                    case "-v":
                        Version v = System.Reflection.Assembly.GetAssembly (typeof (ConsoleMenuController)).GetName ().Version;
                        String installDir = SiteUtils.GetBinPath ();
                        Console.WriteLine ("Version: v{0}.{1}.{2} ", v.Major, v.MinorRevision, v.Revision);
                        Console.WriteLine ("Install Directory: " + installDir);
                        break;
                }
                if (cmd != null)
                    cmd.RunCommand ();
            } else
                throw new Exception (MSG_ERR_BAD_ARGS);
        }

        /// <summary>
        /// Excecutes the given command parameter
        /// </summary>
        /// <param name="args">The arguments used to run the command</param>
        public HelpPointer GetHelpCommand (string[] args) {
            HelpPointer pointer = null;
            if (args.Length > 0 && (args[0] == "" || this.HelpCommands.Contains (args[0]))) {
                CommandController cmd = null;
                String[] cmdArgs;
                cmdArgs = args.Length > 1 ? new String[] { args[1] } : new String[] { "" };
                switch (args[0]) {
                    case "project":
                        cmd = new ProjectController (cmdArgs);
                        break;
                    case "mapping":
                        cmd = new MappingController (cmdArgs);
                        break;
                    case "site":
                        cmd = new SiteController (cmdArgs);
                        break;
                    case "ignore":
                        cmd = new IgnoreController (cmdArgs);
                        break;
                }
                if (cmd != null) {
                    var cmdHelp = cmd.Help;
                    pointer = cmd.Help.FirstOrDefault (x => x.Option == cmd.Option);
                    if (pointer == null)
                        pointer = new HelpPointer (String.Empty, 76, 229);
                } else
                    pointer = new HelpPointer (String.Empty, 76, 229);
                String helpFile = String.Format ("help.{0}.txt", Program.Settings.Language);
                String bin = SiteUtils.GetBinPath ();
                helpFile = Path.Combine (bin, "data", helpFile);
                pointer.SetHelpContent (File.ReadAllLines (helpFile));
                return pointer;
            } else
                throw new Exception (MSG_ERR_BAD_ARGS);

        }

    }
}