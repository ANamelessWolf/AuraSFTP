using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SftpFile = Renci.SshNet.Sftp.SftpFile;
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
        /// Validates if a unix path is valid
        /// </summary>
        /// <param name="unixPath">The unix path to validate</param>
        /// <returns>True if the unix file is valid</returns>
        public bool IsUnixFileValid (string unixPath) {
            String file = unixPath.Substring (unixPath.LastIndexOf ('/'));
            bool isInIgnoreList = this.IgnoreFiles.Contains (file);
            if (isInIgnoreList)
                return false;
            else {
                String ext = file.Contains ('.') ? file.Substring (file.LastIndexOf ('.')) : "";
                return !this.IgnoreExtensions.Contains (ext);
            }
        }
        /// <summary>
        /// Validates if a unix path is valid as a directory 
        /// path
        /// </summary>
        /// <param name="unixFile">The unix path to validate</param>
        /// <returns>True if the unix file is valid</returns>
        public bool IsUnixDirValid (SftpFile unixFile) {
            return unixFile.Name != ".." && unixFile.Name != "." && this.IgnoreDirectories.Contains (unixFile.Name);
        }

        /// <summary>
        /// Filter the directory name
        /// </summary>
        /// <param name="name">The directory name</param>
        /// <returns>True if the directory is not in the ignore list</returns>
        public bool Directory (string name) {
            return !this.IgnoreDirectories.Contains (name);
        }

    }
}