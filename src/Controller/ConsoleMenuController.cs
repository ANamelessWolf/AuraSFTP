using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nameless.Libraries.Aura.Model;
using static Nameless.Libraries.Aura.data.Message;
namespace Nameless.Libraries.Aura.Controller {
    /// <summary>
    /// This class controls the menu controller
    /// </summary>
    public class ConsoleMenuController {
        /// <summary>
        /// Access the application user settings
        /// </summary>
        private UserSettings Settings;
        /// <summary>
        /// The current site credentials
        /// </summary>
        private SiteCredentials Credentials;
        /// <summary>
        /// Defines the console menu valid parameters
        /// </summary>
        private readonly String[] ValidParameters = new String[] { "-p", "-h" };
        /// <summary>
        /// Current Site
        /// </summary>
        public String SiteName { get => this.Settings.Sites[this.Settings.SelectedSite].Site; }
        /// <summary>
        /// Initialize a new instance of the user settings
        /// </summary>
        /// <param name="sett">The application user settings</param>
        public ConsoleMenuController (UserSettings sett) {
            this.Settings = sett;
            int selCred = sett.SelectedSite;
            this.Credentials = sett.Sites[selCred].Data;
        }

        /// <summary>
        /// Gets the console menu
        /// </summary>
        /// <returns>The console menu</returns>
        public string GetMenu () {
            String[] menu = new String[] {
                "Create a new project",
                String.Format ("{0,2} {1,4} {2,9} {3}", "-p", "new", "<prj_name>", "<prj_path>"),
                "Parameters",
                String.Format ("<prj_name> : {0}", "The name of the project"),
                String.Format ("<prj_path> : {0}", "The path where the project is saved"),
                "Example: ",
                "-p new MyProject C:\\MyProjects\\",
            };
            return String.Join ("\n", menu);
        }
        /// <summary>
        /// Excecutes the given command parameter
        /// </summary>
        /// <param name="args">The arguments used to run the command</param>
        public void RunCommand (string[] args) {
            if (args.Length > 0 && this.ValidParameters.Contains (args[0])) {

            } else
                throw new Exception (MSG_ERR_BAD_ARGS);
        }

    }
}