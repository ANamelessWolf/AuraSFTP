using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Nameless.Libraries.Aura.Resources.Message;
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
        protected override string[] ValidOptions => new String[] { "add", "edit", "list", "remove", "test", "default", "check" };
        /// <summary>
        /// Gets the help documentation pointers
        /// </summary>
        public override HelpPointer[] Help => new HelpPointer[] {
            new HelpPointer (this, String.Empty, 1, 74),
            new HelpPointer (this, "add", 87, 94),
            new HelpPointer (this, "edit", 96, 101),
            new HelpPointer (this, "list", 103, 109),
            new HelpPointer (this, "test", 111, 118),
            new HelpPointer (this, "default", 120, 126),
            new HelpPointer (this, "check", 128, 134)
        };
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
                        String fieldName;
                        if (this.Args.Length > 0) {
                            siteName = this.Args[0];
                            fieldName = this.Args.Length > 1 ? this.Args[1] : "";
                            this.EditSite (siteName, fieldName);
                        } else
                            throw new Exception (this.GetErrorArgsMessage (this.Option));
                        break;
                    case "list":
                        siteName = this.Args.Length > 0 ? this.Args[0] : "";
                        this.ListCurrentSites (siteName);
                        break;
                    case "remove":
                        if (this.Args.Length > 0) {
                            siteName = this.Args[0];
                            this.RemoveSite (siteName);
                        } else
                            throw new Exception (this.GetErrorArgsMessage (this.Option));
                        break;
                    case "test":
                        if (this.Args.Length > 0) {
                            siteName = this.Args[0];
                            this.TestSite (siteName);
                        } else
                            throw new Exception (this.GetErrorArgsMessage (this.Option));
                        break;
                    case "default":
                        if (this.Args.Length > 0) {
                            siteName = this.Args[0];
                            this.SetSiteAsDefault (siteName);
                        } else
                            throw new Exception (this.GetErrorArgsMessage (this.Option));
                        break;
                    case "check":
                        this.CheckDefaultConnection ();
                        break;
                } else
                    throw new Exception (String.Format (MSG_ERR_BAD_OPTION, this.CommandShortcut, this.HelpCommand));

        }
        /// <summary>
        /// Checks the site default connection
        /// </summary>
        private void CheckDefaultConnection () {
            if (Program.Settings.Sites.Length == 0)
                Console.WriteLine (MSG_ERR_NO_SITES);
            if (Program.Settings.SelectedSite < Program.Settings.Sites.Length) {
                var site = Program.Settings.Sites[Program.Settings.SelectedSite];
                Console.WriteLine (MSG_INF_SITE_DFTL, site.Site);
                Console.WriteLine (site.ToStringFormat (SiteUtils.ListSizeFormat) + "\n");
            }
        }

        /// <summary>
        /// Sets the given site as the default site connection
        /// </summary>
        /// <param name="siteName">The name of the site</param>
        private void SetSiteAsDefault (string siteName) {
            var site = Program.Settings.Sites.FirstOrDefault (x => x.Site.ToLower () == siteName.ToLower ());
            if (site == null)
                Console.WriteLine (MSG_ERR_SITE_NOT_FOUND, siteName);
            else {
                int index = Program.Settings.Sites.ToList ().IndexOf (site);
                Program.Settings.SelectedSite = index;
                Program.Settings.Save ();
                Console.WriteLine (MSG_INF_SITE_DFTL, siteName);
            }
        }

        /// <summary>
        /// Test the connection to a given site
        /// </summary>
        /// <param name="siteName">The name of the site</param>
        private void TestSite (string siteName) {
            var site = Program.Settings.Sites.FirstOrDefault (x => x.Site.ToLower () == siteName.ToLower ());
            String errMsg;
            if (site == null)
                Console.WriteLine (MSG_ERR_SITE_NOT_FOUND, siteName);
            else {
                if (AuraSftpClient.TestConnection (site.Data, out errMsg))
                    Console.WriteLine (MSG_INF_SITE_CONN_SUCCED, siteName);
                else
                    Console.WriteLine (MSG_INF_SITE_CONN_FAIL, siteName, errMsg);
            }

        }

        /// <summary>
        /// Removes a site from the configuration file
        /// </summary>
        /// <param name="siteName">The site name</param>
        private void RemoveSite (string siteName) {
            var site = Program.Settings.Sites.FirstOrDefault (x => x.Site.ToLower () == siteName.ToLower ());
            if (site == null)
                Console.WriteLine (MSG_ERR_SITE_NOT_FOUND, siteName);
            else {
                Program.Settings.Sites = Program.Settings.Sites.Where (x => x.Site != site.Site).ToArray ();
                Program.Settings.Save ();
                Console.WriteLine (MSG_INF_SITE_REMOVED, siteName);
            }
        }

        /// <summary>
        /// Adds a new site to the configuration file
        /// </summary>
        /// <param name="siteName">The name of the site</param>
        private void EditSite (string siteName, string fieldName = "") {
            var site = Program.Settings.Sites.FirstOrDefault (x => x.Site.ToLower () == siteName.ToLower ());
            if (site == null)
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
                        Console.WriteLine (MSG_INF_SITE_FIELD_UPDATED, siteName, variable.Name);
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