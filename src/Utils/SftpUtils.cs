using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nameless.Libraries.Aura.Controller;
using Nameless.Libraries.Aura.Model;
using Renci.SshNet;
using RenciSftpClient = Renci.SshNet.SftpClient;

using SftpFile = Renci.SshNet.Sftp.SftpFile;
using static Nameless.Libraries.Aura.data.Message;
namespace Nameless.Libraries.Aura.Utils {

    public static class SftpUtils {
        /// <summary>
        /// List the files inside a given directory
        /// </summary>
        /// <param name="client">The Sftp client</param>
        /// <param name="dirPath">The directory path</param>
        /// <returns>The file list collection</returns>
        public static IEnumerable<String> ListFiles (this RenciSftpClient client, string dirPath, SftpFilter filter) {
            List<String> files = new List<String> ();
            IEnumerable<String> result;
            var entries = client.ListDirectory (dirPath);
            foreach (var entry in entries.Where (x => !x.IsDirectory))
            if(filter.UnixFile())
                files.Add (entry.FullName);
            result = files;
            foreach (var subDirPath in entries.Where (x => x.IsDirectory && x.Name != ".." && x.Name != "."))
                result = result.Union (ListFiles (client, subDirPath.FullName, filter));
            return result;
        }
        /// <summary>
        /// Downloads a directory
        /// </summary>
        /// <param name="client">The Sftp client</param>
        /// <param name="dir">The directory used to download its files</param>
        /// <param name="filter">The download filter</param>
        public static void Download (this RenciSftpClient client, SiteCredentials credentials, MappedPath dir, SftpFilter filter, Boolean replace) {
            var files = SftpUtils.ListFiles (client, dir.RemotePath, filter);
            string localPath, serverCopy;
            FileInfo sC, lC;
            AuraSftpClient.SSHTransactionVoid (credentials, (AuraSftpClient c) => {
                foreach (String file in files) {
                    localPath = file.Replace (dir.RemotePath, "").Substring (1).Replace ("/", "\\");
                    serverCopy = Path.Combine (dir.ServerCopy, localPath);
                    sC = new FileInfo (serverCopy);
                    lC = new FileInfo (Path.Combine (dir.ProjectCopy, localPath));
                    DownloadFile (c, file, sC, lC, replace);
                }
            });
        }
        /// <summary>
        /// Dowloads a file using a SFTP client
        /// </summary>
        /// <param name="client">The SFTP client</param>
        /// <param name="remoteFile">The remote file path</param>
        /// <param name="serverCopy">The server file copy</param>
        /// <param name="localCopy">The project file copy</param>
        /// <param name="replace">True if the file is replaced</param>
        private static void DownloadFile (AuraSftpClient client, string remoteFile, FileInfo serverCopy, FileInfo localCopy, Boolean replace) {
            Console.WriteLine (String.Format (MSG_INF_DOW_FILE, remoteFile));

            if (!Directory.Exists (serverCopy.Directory.FullName))
                Directory.CreateDirectory (serverCopy.Directory.FullName);
            client.Download (remoteFile, serverCopy);

            if (!Directory.Exists (localCopy.Directory.FullName))
                Directory.CreateDirectory (localCopy.Directory.FullName);
            if (File.Exists (localCopy.FullName) && !replace)
                Console.WriteLine (String.Format (MSG_INF_EXIST_OMIT_FILE, localCopy.FullName));
            else {
                File.Copy (serverCopy.FullName, localCopy.FullName, replace);
                if (!File.Exists (localCopy.FullName))
                    Console.WriteLine (String.Format (MSG_INF_COPY_FILE, localCopy.FullName));
                else
                    Console.WriteLine (String.Format (MSG_INF_EXIST_REPLACE_FILE, localCopy.FullName));
            }
        }
    }
}