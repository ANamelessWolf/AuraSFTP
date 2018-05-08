using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Nameless.Libraries.Aura.data.Message;
using Nameless.Libraries.Aura.Model;
using Nameless.Libraries.Aura.Utils;
using RenciSftpClient = Renci.SshNet.SftpClient;
using System.Text;

namespace Nameless.Libraries.Aura.Controller {

    public class SiteController : CommandController {
        /// <summary>
        /// The command shortcut
        /// </summary>
        /// <returns>The command shortcut</returns>
        public override string CommandShortcut => "-s";
        /// <summary>
        /// The help command
        /// </summary>
        public override string HelpCommand => "site";
        /// <summary>
        /// The validation options
        /// </summary>
        protected override string[] ValidOptions => new String[] { "add", "edit", "list" };
        /// <summary>
        /// Initialize a new instance for a command controller
        /// </summary>
        /// <param name="args">The command arguments</param>
        public SiteController (string[] args) : base (args) { }
        /// <summary>
        /// Runs the given command
        /// <throw>An exception is thrown when the given option is invalid</throw>
        /// </summary>
        public override void RunCommand () {
            if (this.ValidOptions.Contains (this.Option))
                switch (this.Option) {
                    case "add":
                        this.AddSite ();
                        break;
                    case "list":
                        String siteName = this.Args.Length > 0 ? this.Args[0] : "";
                        this.ListCurrentSites (siteName);
                        break;
                } else
                    throw new Exception (String.Format (MSG_ERR_BAD_OPTION, this.CommandShortcut, this.HelpCommand));

        }
        /// <summary>
        /// Adds a new site to the configuration file
        /// </summary>
        private void AddSite () {
            String siteName = CommandUtils.Ask ("Site Name?");
            if (Program.Settings.Sites.Count (x => x.Site.ToLower () == siteName.ToLower ()) == 0) {
                var variables = typeof (SiteCredentials).GetFields ();
                SiteCredentials credentials = new SiteCredentials ();
                Object value;
                foreach (var v in variables) {

                    int port;
                    if (v.Name == "Password")
                        value = CommandUtils.AskPassword ("User " + v.Name);
                    else
                        value = CommandUtils.Ask ("Value of " + v.Name);
                    if (v.FieldType == typeof (int))
                        value = int.TryParse (value.ToString (), out port) ? port : 22;
                    v.SetValue (credentials, value);
                }
                if (SiteUtils.IsValid (credentials)) {
                    Program.Settings.Sites = Program.Settings.Sites.Union (new SiteDefinition[] {
                        new SiteDefinition () { Site = siteName, Data = credentials }
                    }).ToArray ();
                    Program.Settings.Save ();
                    Console.WriteLine (MSG_INF_SITE_ADDED, siteName);
                } else
                    Console.WriteLine (MSG_ERR_BAD_CRED);
            } else
                Console.WriteLine (MSG_ERR_SITE_EXISTS, siteName);
        }

        /// <summary>
        /// List the current sites
        /// </summary>
        /// <param name="siteName">The available site names</param>
        private void ListCurrentSites (string siteName) {
            var sites = Program.Settings.Sites;
            StringBuilder output = new StringBuilder ();
            String format = SiteUtils.ListSizeFormat;
            if (siteName == "" && sites.Length > 0)
                sites.ToList ().ForEach (x => output.Append (x.ToStringFormat (format) + "\n"));
            else if (sites.Length == 0)
                output.Append (MSG_ERR_NO_SITES);
            else {
                var site = sites.FirstOrDefault (x => x.Site.ToLower () == siteName.ToLower ());
                if (site != null)
                    output.Append (site.ToStringFormat (format));
                else
                    output.Append (String.Format (MSG_ERR_SITE_NOT_FOUND, siteName));
            }
            Console.WriteLine (output.ToString ());
        }
    }
}