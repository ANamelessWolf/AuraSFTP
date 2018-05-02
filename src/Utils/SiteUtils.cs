using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nameless.Libraries.Aura.Model;
using Newtonsoft.Json;

namespace Nameless.Libraries.Aura.Utils {

    public static class SiteUtils {
        public static SiteCredentials GetCredentialsBySelectedIndex (int selIndex) {
            return null;
        }

        public static UserSettings GetUserSettings () {
            UserSettings settings=null;
            string pth = Path.Combine ("data", "settings.json");
            using (StreamReader r = new StreamReader (pth)) {
                string json = r.ReadToEnd ();
                settings = JsonConvert.DeserializeObject<UserSettings> (json);
            }
            return settings;
        }
    }
}