using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nameless.Libraries.Aura.Model;
using Newtonsoft.Json;

namespace Nameless.Libraries.Aura.Utils {

    public static class SiteUtils {

        /// <summary>
        /// Get the current application settings
        /// </summary>
        /// <returns>The SSH app settings</returns>
        public static UserSettings GetUserSettings () {
            UserSettings settings = null;
            String bin = System.Reflection.Assembly.GetAssembly (typeof (SiteUtils)).Location;
            bin = new FileInfo (bin).Directory.FullName;
            string pth = Path.Combine (bin, "data", "settings.json");
            using (StreamReader r = new StreamReader (pth)) {
                string json = r.ReadToEnd ();
                settings = JsonConvert.DeserializeObject<UserSettings> (json);
            }
            return settings;
        }
    }
}