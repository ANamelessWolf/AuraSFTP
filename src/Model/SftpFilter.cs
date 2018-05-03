using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nameless.Libraries.Aura.Model {

    public class SftpFilter {
        /// <summary>
        /// The collection of extension files to ignore
        /// </summary>
        public String[] IgnoreExtensions;
        /// <summary>
        /// The collection of files to ignore
        /// </summary>
        public String[] IgnoreFiles;
        /// <summary>
        /// The collection of directories to ignore
        /// </summary>
        public String[] IgnoreDirectories;
        /// <summary>
        /// Initialize a new instance of a SftpFilter
        /// </summary>
        /// <param name="prj">The current project</param>
        public SftpFilter (Project prj) {
            this.IgnoreDirectories = prj.Data.IgnoreDirectories;
            this.IgnoreFiles = prj.Data.IgnoreFiles;
            this.IgnoreExtensions = prj.Data.IgnoreExtensions;
        }
        /// <summary>
        /// Filter the directory name
        /// </summary>
        /// <param name="name">The directory name</param>
        /// <returns>True if the directory is not in the ignore list</returns>
        public bool Directory (string name) {
            return !this.IgnoreDirectories.Contains(name);
        }
    }
}