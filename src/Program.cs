using System;
using Nameless.Libraries.Aura.Controller;
using Nameless.Libraries.Aura.Model;
using Nameless.Libraries.Aura.Utils;

namespace Nameless.Libraries.Aura {
    class Program {
        static void Main (string[] args) {
            UserSettings sett = SiteUtils.GetUserSettings ();
            ConsoleMenuController menu = new ConsoleMenuController (sett);
            menu.RunCommand (args);

            // SftpClient.SSHTransactionVoid (site, (SftpClient client) => {
            //     foreach (var item in client.ListFiles ("/var/www/html/geocloud/protected/controllers"))
            //         Console.WriteLine (item);

            // });
        }
    }
}