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
        private readonly String[] ValidParameters = new String[] { "-p", "-h", "i", "-m", "-s", "-v" };
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
            } else
                throw new Exception (MSG_ERR_NO_SITES);

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
                "Pull from server",
                "Downloads the mapped files from the server",
                String.Format ("{0,2} {1,4} {2,9} {3}", "-p", "pull", "replace", "[optional]"),
                "Parameters",
                String.Format ("[optional]replace : {0}", "Use the word 'replace' to overwrite existant files"),
                "Example: ",
                "-p pull replace",
                "Map a directory",
                "Maps a directory from the server remote path to the local path. The directory is mapped with subdirectory recursion",
                String.Format ("{0,2} {1,4} {2,9} {3}", "-m", "dir", "<local_path>", "<remote_path>"),
                "Parameters",
                String.Format ("<local_path> : {0}", "The local path for the directory, use the project path as base directory"),
                String.Format ("<remote_path> : {0}", "The remote path where the directory is downloaded use the connection RootDir as base path"),
                "Example: ",
                "-m dir JavaScript\\Widget\\downloader JavaScript/Widget/downloader",
                "Map a directory",
                "Maps a file from the server remote path to the local path. ",
                String.Format ("{0,2} {1,4} {2,9} {3}", "-m", "file", "<local_file_path>", "<remote_file_path>"),
                "Parameters",
                String.Format ("<local_file_path> : {0}", "The local path for the file, use the project path as base file"),
                String.Format ("<remote_file_path> : {0}", "The remote path where the file is downloaded use the connection RootDir as base path"),
                "Example: ",
                "-m file protected\\config\\properties.php config/properties.php",
                "Removes a path",
                "Removes a path from the current Project",
                String.Format ("{0,2} {1,4} {2,9}", "-m", "remove", "<local_path>"),
                "Parameters",
                String.Format ("<local_path> : {0}", "The local path to remove"),
                "Example: ",
                "-m file protected\\config\\properties.php config/properties.php",
            };
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
                        String installDir =System.Reflection.Assembly.GetAssembly (typeof (SiteUtils)).Location;
                        installDir = new FileInfo (installDir).Directory.FullName;
                        Console.WriteLine ("Version: v{0}.{1}.{2} " , v.Major, v.MinorRevision, v.Revision);
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