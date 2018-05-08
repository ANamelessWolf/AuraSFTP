using System;
using Nameless.Libraries.Aura.Controller;
using Nameless.Libraries.Aura.Model;
using Nameless.Libraries.Aura.Utils;

namespace Nameless.Libraries.Aura {
    class Program {
        /// <summary>
        /// Defines the application user settings
        /// </summary>
        public static UserSettings Settings;
        static void Main (string[] args) {
            Settings = SiteUtils.GetUserSettings ();
            ConsoleMenuController menu = new ConsoleMenuController (Settings);
            try {
                menu.RunCommand (args);
            } catch (System.Exception exc) {
                Console.WriteLine (exc.Message);
            }
        }

        // SftpClient.SSHTransactionVoid (site, (SftpClient client) => {
        //     foreach (var item in client.ListFiles ("/var/www/html/geocloud/protected/controllers"))
        //         Console.WriteLine (item);

        // });
    }
}