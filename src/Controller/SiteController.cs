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
            String siteName;
            if (this.ValidOptions.Contains (this.Option))
                switch (this.Option) {
                    case "add":
                        siteName = this.Args.Length > 0 ? this.Args[0] : "";
                        this.AddSite (siteName);
                        break;
                    case "edit":
                        siteName = this.Args.Length > 0 ? this.Args[0] : "";
                        this.EditSite (siteName);
                        break;
                    case "list":
                        siteName = this.Args.Length > 0 ? this.Args[0] : "";
                        this.ListCurrentSites (siteName);
                        break;
                } else
                    throw new Exception (String.Format (MSG_ERR_BAD_OPTION, this.CommandShortcut, this.HelpCommand));

        }
        /// <summary>
        /// Adds a new site to the configuration file
        /// </summary>
        /// <param name="siteName">The name of the site</param>
        private void EditSite (string siteName, string fieldName = "") {
            var site = Program.Settings.Sites.FirstOrDefault (x => x.Site.ToLower () == siteName.ToLower ());
            if (site != null)
                Console.WriteLine (MSG_ERR_SITE_NOT_FOUND, siteName);
            else {
                if (fieldName == "") { //Update all fields
                    if (site.Data.UpdateData ()) {
                        Program.Settings.Save ();
                        Console.WriteLine (MSG_INF_SITE_UPDATED, siteName);
                    } else
                        Console.WriteLine (MSG_ERR_BAD_CRED);
                } else {
                    var variable = typeof (SiteCredentials).GetFields ().FirstOrDefault (x => x.Name.ToLower () == fieldName.ToLower ());
                    if (variable != null) {
                        site.Data.UpdateField (variable);
                        Program.Settings.Save ();
                    } else
                        Console.WriteLine (MSG_ERR_SITE_UPDATED, siteName, fieldName);
                }
            }
        }

        /// <summary>
        /// Adds a new site to the configuration file
        /// </summary>
        /// <param name="siteName">The name of the site</param>
        private void AddSite (String siteName = "") {
            siteName = siteName == "" ? CommandUtils.Ask ("Site Name?") : siteName;
            if (Program.Settings.Sites.Count (x => x.Site.ToLower () == siteName.ToLower ()) == 0) {
                SiteCredentials credentials = new SiteCredentials ();
                if (credentials.UpdateData ()) {
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
        /// <param name="siteName">The name of the site</param>
        private void ListCurrentSites (string siteName = "") {
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