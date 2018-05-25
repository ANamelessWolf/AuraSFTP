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

    }
}