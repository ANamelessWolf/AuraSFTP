using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nameless.Libraries.Aura.Model;
using RenciSftpClient = Renci.SshNet.SftpClient;
using SftpFile = Renci.SshNet.Sftp.SftpFile;
namespace Nameless.Libraries.Aura.Utils {

    public static class SftpUtils {
        /// <summary>
        /// List the files inside a given directory
        /// </summary>
        /// <param name="client">The Sftp client</param>
        /// <param name="dirPath">The directory path</param>
        /// <returns>The file list collection</returns>
        public static IEnumerable<String> ListFiles (this RenciSftpClient client, string dirPath) {
            List<String> files = new List<String> ();
            IEnumerable<String> result;
            var entries = client.ListDirectory (dirPath);
            foreach (var entry in entries.Where (x => !x.IsDirectory))
                files.Add (entry.FullName);
            result = files;
            foreach (var subDirPath in entries.Where (x => x.IsDirectory && x.Name != ".." && x.Name != "."))
                result = result.Union (ListFiles (client, subDirPath.FullName));
            return result;
        }
        /// <summary>
        /// Download all files from a remote directory in the local directory
        /// </summary>
        /// <param name="client">The Sftp client</param>
        /// <param name="entry">The Sftp directory entry</param>
        /// <param name="project">The active project</param>
        /// <param name="rootDir">The root directory</param>
        public static IEnumerable<MappedPath> MapDirectory (this RenciSftpClient client, SftpFile entry, Project project, SftpFilter filter, String rootDir, String localDir) {
            //List<String> files = new List<String> ();
            //var directories = client.ListDirectory (entry.FullName).Where (x => x.IsDirectory && filter.Directory (x.Name) && x.Name != ".." && x.Name != ".");
            // var files = client.ListFiles(entry.FullName).Where()
            return new MappedPath[0];

        }
    }
}