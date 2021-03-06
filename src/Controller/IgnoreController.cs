using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nameless.Libraries.Aura.Model;
using Nameless.Libraries.Aura.Utils;
using static Nameless.Libraries.Aura.Resources.Message;
using static Nameless.Libraries.Aura.Utils.CommandUtils;

namespace Nameless.Libraries.Aura.Controller {

    public class IgnoreController : CommandController {
        /// <summary>
        /// The path to the configuration file
        /// </summary>
        public String ConfigFile;
        /// <summary>
        /// Command shortcut
        /// </summary>
        public override string CommandShortcut => "-i";
        /// <summary>
        /// Gets the help command name
        /// </summary>
        public override string HelpCommand => "ignore";
        /// <summary>
        /// The command valid options
        /// </summary>
        protected override string[] ValidOptions => new String[] { "dir", "file", "ext", "remove", "list" };
        /// <summary>
        /// The command sub valid options
        /// </summary>
        private string[] SubValidOptions => new String[] { "dir", "file", "ext" };
        /// <summary>
        /// Gets the help documentation pointers
        /// </summary>
        public override HelpPointer[] Help => new HelpPointer[] {
            new HelpPointer (this, String.Empty, 178, 229),
            new HelpPointer (this, "dir", 188, 194),
            new HelpPointer (this, "file", 196, 202),
            new HelpPointer (this, "ext", 204, 210),
            new HelpPointer (this, "remove", 212, 220),
            new HelpPointer (this, "list", 222, 229)
        };
        /// <summary>
        /// Initialize a new instance for a project controller
        /// </summary>
        /// <param name="args">The command arguments</param>
        public IgnoreController (string[] args) : base (args) { }
        /// <summary>
        /// Runs the given command
        /// <throw>An exception is thrown when the given option is invalid</throw>
        /// </summary>
        public override void RunCommand () {
            String localPath = Environment.CurrentDirectory;
            this.ConfigFile = Path.Combine (localPath, ".ssh", "config.json");
            Boolean optionsAreValid = this.ValidOptions.Contains (this.Option),
                sshProjectExists = File.Exists (this.ConfigFile);
            if (optionsAreValid && sshProjectExists) {
                Project prj = this.ConfigFile.OpenProjectFile ();
                try {
                    switch (this.Option) {
                        case "dir":
                            if (this.Args.Length == 1)
                                this.IgnoreDirectory (prj, this.Args[0]);
                            else
                                throw new Exception (this.GetErrorArgsMessage (this.Option));
                            break;
                        case "file":
                            if (this.Args.Length == 1)
                                this.IgnoreFile (prj, this.Args[0]);
                            else
                                throw new Exception (this.GetErrorArgsMessage (this.Option));
                            break;
                        case "ext":
                            if (this.Args.Length > 1)
                                this.IgnoreExtension (prj, this.Args);
                            else
                                throw new Exception (this.GetErrorArgsMessage (this.Option));
                            break;

                        case "remove":
                            if (this.Args.Length == 2)
                                this.Remove (prj, this.Args[0], this.Args[1]);
                            else
                                throw new Exception (this.GetErrorArgsMessage (this.Option));
                            break;
                        case "list":
                            if (this.Args.Length == 1)
                                this.ListIgnore (prj, this.Args[0]);
                            else
                                throw new Exception (this.GetErrorArgsMessage (this.Option));
                            break;
                    }
                } catch (Exception exc) {
                    throw exc;
                }
            } else if (!sshProjectExists)
                throw new Exception (MSG_ERR_PRJ_MISS);
            else if (!optionsAreValid)
                throw new Exception (String.Format (MSG_ERR_BAD_OPTION, this.CommandShortcut, this.HelpCommand));
        }
        /// <summary>
        /// List the entries of an ignore list
        /// </summary>
        /// <param name="prj">The active project</param>
        /// <param name="optionToList">The selected list option, file|ext|dir</param>
        private void ListIgnore (Project prj, string optionToList) {
            if (this.SubValidOptions.Contains (optionToList)) {
                String[] list = new String[0];
                String join = ", ";
                switch (optionToList) {
                    case "dir":
                        list = prj.Data.IgnoreDirectories;
                        break;
                    case "file":
                        list = prj.Data.IgnoreFiles;
                        break;
                    case "ext":
                        list = prj.Data.IgnoreExtensions;
                        break;
                }
                Console.WriteLine (String.Join (join, list));
            } else
                throw new Exception (String.Format (MSG_ERR_BAD_OPTION, this.CommandShortcut, this.HelpCommand));
        }

        /// <summary>
        /// Removes an entry from a ignore list
        /// </summary>
        /// <param name="prj">The active project</param>
        /// <param name="optionToRemove">The selected remove option, file|ext|dir</param>
        /// <param name="value">The value to remove</param>
        private void Remove (Project prj, string optionToRemove, string value) {
            if (this.SubValidOptions.Contains (optionToRemove)) {
                switch (optionToRemove) {
                    case "dir":
                        if (prj.Data.IgnoreDirectories.Select (x => x.ToLower ()).Contains (value))
                            prj.Data.IgnoreDirectories = prj.Data.IgnoreDirectories.Where (x => x.ToLower () != value).ToArray ();
                        else
                            Console.WriteLine (MSG_ERR_IGNORE_NOT_DEF, value, "directories");
                        break;
                    case "file":
                        if (prj.Data.IgnoreFiles.Select (x => x.ToLower ()).Contains (value))
                            prj.Data.IgnoreFiles = prj.Data.IgnoreFiles.Where (x => x.ToLower () != value).ToArray ();

                        else
                            Console.WriteLine (MSG_ERR_IGNORE_NOT_DEF, value, "files");
                        break;
                    case "ext":
                        if (prj.Data.IgnoreExtensions.Select (x => x.ToLower ()).Contains (value))
                            prj.Data.IgnoreExtensions = prj.Data.IgnoreExtensions.Where (x => x.ToLower () != value).ToArray ();
                        else
                            Console.WriteLine (MSG_ERR_IGNORE_NOT_DEF, value, "extensions");
                        break;
                }
                prj.SaveProject (this.ConfigFile);
            } else
                throw new Exception (String.Format (MSG_ERR_BAD_OPTION, this.CommandShortcut, this.HelpCommand));
        }

        /// <summary>
        /// Adds an extension to the ignore list of the project
        /// </summary>
        /// <param name="prj">The active project</param>
        /// <param name="extensions">The extension to ignore</param>
        private void IgnoreExtension (Project prj, params string[] extensions) {
            if (extensions.Count (x => x.Contains ('.')) != extensions.Count ())
                throw new Exception (MSG_ERR_BAD_EXT);
            else if (extensions.Count (x => !prj.Data.IgnoreExtensions.Contains (x)) == 0)
                throw new Exception (MSG_ERR_IGNORE_EXIST);
            else {
                String addedExt = String.Empty;
                List<String> newExtensions = new List<string> ();
                foreach (string ext in extensions) {
                    newExtensions.Add (ext);
                    addedExt += ext + ", ";
                }
                prj.Data.IgnoreExtensions = prj.Data.IgnoreExtensions.Union (newExtensions).ToArray ();
                Console.WriteLine (MSG_INF_IGNORE_ADDED, addedExt.Substring (0, addedExt.Length - 2), "extensions");
                prj.SaveProject (this.ConfigFile);
            }
        }
        /// <summary>
        /// Adds a file to the ignore list of the project
        /// </summary>
        /// <param name="prj">The active project</param>
        /// <param name="fileName">The file name</param>
        private void IgnoreFile (Project prj, string fileName) {
            if (prj.Data.IgnoreFiles.Contains (fileName))
                throw new Exception (MSG_ERR_IGNORE_EXIST);
            else {
                prj.Data.IgnoreFiles = prj.Data.IgnoreFiles.Union (new String[] { fileName }).ToArray ();
                prj.SaveProject (this.ConfigFile);
                Console.WriteLine (MSG_INF_IGNORE_ADD, fileName, "files");
            }
        }

        /// <summary>
        /// Adds a directory to the ignore list of the project
        /// </summary>
        /// <param name="prj">The active project</param>
        /// <param name="dirName">The directory name</param>
        private void IgnoreDirectory (Project prj, string dirName) {
            if (prj.Data.IgnoreDirectories.Contains (dirName))
                throw new Exception (MSG_ERR_IGNORE_EXIST);
            else {
                prj.Data.IgnoreDirectories = prj.Data.IgnoreDirectories.Union (new String[] { dirName }).ToArray ();
                prj.SaveProject (this.ConfigFile);
                Console.WriteLine (MSG_INF_IGNORE_ADD, dirName, "directory");
            }

        }
    }
}