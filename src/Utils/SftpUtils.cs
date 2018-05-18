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
using Renci.SshNet.Common;

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
                if (filter.IsUnixFileValid (entry.FullName))
                    files.Add (entry.FullName);
            result = files;
            foreach (var subDirPath in entries.Where (x => x.IsDirectory && filter.IsUnixDirValid (x)))
                result = result.Union (ListFiles (client, subDirPath.FullName, filter));
            return result;
        }
        /// <summary>
        /// Downloads a directory
        /// </summary>
        /// <param name="client">The Sftp client</param>
        /// <param name="file">The directory used to download its files</param>
        /// <param name="filter">The download filter</param>
        public static void Download (this RenciSftpClient client, SiteCredentials credentials, MappedPath dir, SftpFilter filter, Boolean replace) {
            var files = SftpUtils.ListFiles (client, dir.RemotePath, filter);
            string localPath, serverCopy;
            FileInfo sC, lC;
            AuraSftpClient.SSHTransactionVoid (credentials, (Action<AuraSftpClient>) ((AuraSftpClient c) => {
                foreach (String file in files) {
                    localPath = file.Replace ((string) dir.RemotePath, "").Substring (1).Replace ("/", "\\");
                    serverCopy = Path.Combine ((string) dir.ServerCopy, localPath);
                    sC = new FileInfo (serverCopy);
                    lC = new FileInfo (Path.Combine ((string) dir.ProjectCopy, localPath));
                    DownloadFile (c, file, sC, lC, replace);
                }
            }));
        }
        /// <summary>
        /// Downloads a file 
        /// </summary>
        /// <param name="credentials">The ssh site credentials</param>
        /// <param name="fileMap">The mapping file to download</param>
        /// <param name="replace">True if the downloaded file will be replaced</param>
        public static void Download (this SiteCredentials credentials, MappedPath fileMap, Boolean replace) {
            FileInfo sC, lC;
            AuraSftpClient.SSHTransactionVoid (credentials, (AuraSftpClient c) => {
                sC = new FileInfo (fileMap.ServerCopy);
                lC = new FileInfo (fileMap.ProjectCopy);
                DownloadFile (c, fileMap.RemotePath, sC, lC, replace);
            });
        }
        /// <summary>
        /// Gets a mapped path from a file
        /// </summary>
        /// <param name="file">The file to map</param>
        /// <param name="projectPath">The project path</param>
        /// <param name="remotePath">The remote path</param>
        /// <returns>The mapped path</returns>
        public static MappedPath GetMappedPath (FileInfo file, String projectPath, String remotePath, String serverPth) {
            return new MappedPath () {
                ProjectCopy = file.FullName,
                    RemotePath = GetServerPath (file.FullName.Replace (projectPath, ""), remotePath),
                    ServerCopy = file.FullName.Replace (projectPath, serverPth)
            };
        }
        /// <summary>
        /// Gets the path to the file in unix format
        /// </summary>
        /// <param name="filePath">The relative path</param>
        /// <param name="remotePath">The file remote path</param>
        /// <returns>The server path</returns>
        private static string GetServerPath (string filePath, string remotePath) {
            return remotePath + filePath.Replace ('\\', '/');
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
        /// <summary>
        /// Uploads a collection of files to the current server site
        /// </summary>
        /// <param name="files">The collection of files to upload</param>
        /// <param name="project">The active project</param>
        public static void UploadFiles (IEnumerable<MappedPath> files, Project project) {
            Console.WriteLine (MSG_TIT_FILES_UP);
            files.ToList ().ForEach (x => Console.WriteLine (x.ToUploadPreviewFormat ()));
            if (CommandUtils.AskYesNo (MSG_ASK_CONTINUE)) {
                AuraSftpClient.SSHTransactionVoid (project.Connection.Data,
                    (Action<AuraSftpClient>) ((AuraSftpClient c) => {
                        c.Uploading += (object sender, ScpUploadEventArgs e) => {
                            Console.WriteLine (String.Format ("\rUploading [{0}], {1:P2}", e.Filename,
                                (double) e.Uploaded / (double) e.Size));
                        };
                        foreach (MappedPath file in files.OrderBy (x => x.RemotePath)) {
                            c.Upload (new FileInfo (file.ProjectCopy), file.RemotePath);
                            Console.WriteLine (String.Format ("Uploaded at {0}", file.RemotePath));
                        }

                    }));
            }
        }

    }
}