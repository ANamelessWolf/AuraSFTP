using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Nameless.Libraries.Aura.Model;
using Newtonsoft.Json;
using static Nameless.Libraries.Aura.Resources.Message;
namespace Nameless.Libraries.Aura.Utils {

    public static class SiteUtils {
        /// <summary>
        /// Gets the format of a site used when a site is listed
        /// </summary>
        /// <returns>The site listed format</returns>
        public static String ListSizeFormat {
            get => "Site {0}\n" +
                "\tSite directory: {1}\n" +
                "\tHost: {2}\n" +
                "\tUser: {3}\n" +
                "\tPassword: {4}\n" +
                "\tPort: {5}\n";
        }
        /// <summary>
        /// Get the current application settings
        /// </summary>
        /// <returns>The SSH app settings</returns>
        public static UserSettings GetUserSettings () {
            UserSettings settings = null;
            String bin = GetBinPath ();
            string pth = Path.Combine (bin, "data", "settings.json");
            if (File.Exists (pth)) {
                using (StreamReader r = new StreamReader (pth)) {
                    string json = r.ReadToEnd ();
                    settings = JsonConvert.DeserializeObject<UserSettings> (json);
                }
                return settings;
            } else throw new Exception (MSG_ERR_APP_MISS_CONF);
        }
        /// <summary>
        /// Gets the bin remote folder path
        /// </summary>
        /// <returns>The path where the application is installed</returns>
        public static string GetBinPath () {
            String bin = System.Reflection.Assembly.GetAssembly (typeof (SiteUtils)).Location;
            return new FileInfo (bin).Directory.FullName;
        }

        /// <summary>
        /// Gets a string, formatting the site credentials
        /// </summary>
        /// <param name="site">The site to get a formatted string</param>
        /// <param name="format">The site format</param>
        /// <returns>The site format string</returns>
        public static string ToStringFormat (this SiteDefinition site, String format) {
            return String.Format (format,
                site.Site,
                site.Data.RootDir,
                site.Data.Host,
                site.Data.User,
                "********",
                site.Data.Port);
        }
        /// <summary>
        /// Validates that the site credentials are valid
        /// </summary>
        /// <param name="credentials">The site credentials</param>
        /// <returns>True if the site credentials are valid</returns>
        public static bool IsValid (this SiteCredentials credentials) {
            if (credentials.Port == 0)
                credentials.Port = 22;
            return credentials.Host.Length > 0 && credentials.User.Length > 0 && credentials.RootDir.Length > 0;
        }
        /// <summary>
        /// Saves the current settings
        /// </summary>
        /// <param name="settings">The current application settings</param>
        public static void Save (this UserSettings settings) {
            String bin = System.Reflection.Assembly.GetAssembly (typeof (SiteUtils)).Location;
            bin = new FileInfo (bin).Directory.FullName;
            string pth = Path.Combine (bin, "data", "settings.json");
            File.WriteAllText (pth, JsonConvert.SerializeObject (settings));
        }
        /// <summary>
        /// Updates the site data via the user request
        /// </summary>
        /// <param name="site">The site to update</param>
        /// <returns>True if the user updated data is valid</returns>
        public static Boolean UpdateData (this SiteCredentials site) {
            var variables = typeof (SiteCredentials).GetFields ();
            foreach (var v in variables)
                UpdateField (site, v);
            return SiteUtils.IsValid (site);
        }
        /// <summary>
        /// Update the given field
        /// </summary>
        /// <param name="site">The site to update</param>
        /// <param name="field">The field to update</param>
        public static void UpdateField (this SiteCredentials site, FieldInfo field) {
            Object value;
            int port;
            if (field.Name == "Password")
                value = CommandUtils.AskPassword ("User " + field.Name).Replace("\r","");
            else
                value = CommandUtils.Ask ("Value of " + field.Name);
            if (field.FieldType == typeof (int))
                value = int.TryParse (value.ToString (), out port) ? port : 22;
            field.SetValue (site, value);
        }
    }
}