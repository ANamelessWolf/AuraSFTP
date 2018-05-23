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
            return unixFile.Name != ".." && unixFile.Name != "." && !this.IgnoreDirectories.Contains (unixFile.Name);
        }

        /// <summary>
        /// Gets the file paths in the given directory
        /// </summary>
        /// <param name="localPath">The directory local path</param>
        /// <returns>The files in the directory</returns>
        public IEnumerable<string> FilesInDirectory (string localPath) {
            DirectoryInfo info = new DirectoryInfo (localPath);
            var files = new List<FileInfo> ();
            this.GetFiles (ref files, info);
            return files.Select (x => x.FullName);
        }
        /// <summary>
        /// Check if a file is ignore by its file extension
        /// </summary>
        /// <param name="file">The file to validate</param>
        /// <returns>True if the file is ignored</returns>
        public Boolean IsIgnoreExtension (FileInfo file) {
            return this.IgnoreExtensions.Select (x => x.ToLower ()).Contains (file.Extension.ToLower ());
        }
        /// <summary>
        /// Check if a file is ignore by its name
        /// </summary>
        /// <param name="file">The file to validate</param>
        /// <returns>True if the file is ignored</returns>
        public Boolean IsIgnoreFile (FileInfo file) {
            return this.IgnoreFiles.Select (x => x.ToLower ()).Contains (file.Name.ToLower ());
        }
        /// <summary>
        /// Check if the directory name is in the ignore list
        /// </summary>
        /// <param name="dir">The directory to validate</param>
        /// <returns>True if the directory is ignored</returns>
        public Boolean IsIgnoreDirectory (DirectoryInfo dir) {
            return this.IgnoreFiles.Select (x => x.ToLower ()).Contains (dir.Name.ToLower ());
        }
        /// <summary>
        /// Gets the files in a given directory
        /// </summary>
        /// <param name="files">The extracted files</param>
        /// <param name="info">The directory info</param>
        public void GetFiles (ref List<FileInfo> files, DirectoryInfo info) {
            foreach (FileInfo file in info.GetFiles ().Where (x => !IsIgnoreExtension (x) && !IsIgnoreFile (x)))
                files.Add (file);
            foreach (DirectoryInfo dir in info.GetDirectories ().Where (x => !IsIgnoreDirectory (x)))
                GetFiles (ref files, dir);
        }
    }
}