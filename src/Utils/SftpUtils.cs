using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nameless.Libraries.Aura.Controller;
using Nameless.Libraries.Aura.Model;
using Renci.SshNet;
using RenciSftpClient = Renci.SshNet.SftpClient;

using SftpFile = Renci.SshNet.Sftp.SftpFile;
using static Nameless.Libraries.Aura.Resources.Message;
using Renci.SshNet.Common;

namespace Nameless.Libraries.Aura.Utils {

    public static class SftpUtils {
        /// <summary>
        /// List the files inside a given directory
        /// </summary>
        /// <param name="client">The Sftp client</param>
        /// <param name="dirPath">The directory path</param>
        /// <param name="silentDownload">Download the files without listing everything</param>
        /// <returns>The file list collection</returns>
        public static IEnumerable<String> ListFiles (this RenciSftpClient client, string dirPath, SftpFilter filter, Boolean silentDownload = false) {
            List<String> files = new List<String> ();
            IEnumerable<String> result;
            var entries = client.ListDirectory (dirPath);
            if (silentDownload)
                Console.WriteLine ("Downloading {0} entries...", entries.Count ());
            foreach (var entry in entries.Where (x => !x.IsDirectory))
                if (filter.IsUnixFileValid (entry.FullName))
                    files.Add (entry.FullName);
            result = files;
            foreach (var subDirPath in entries.Where (x => x.IsDirectory && filter.IsUnixDirValid (x))) {
                Console.WriteLine (subDirPath);
                result = result.Union (ListFiles (client, subDirPath.FullName, filter, silentDownload));
            }
            return result;
        }
        /// <summary>
        /// Downloads a directory
        /// </summary>
        /// <param name="client">The Sftp client</param>
        /// <param name="file">The directory used to download its files</param>
        /// <param name="filter">The download filter</param>
        /// <param name="silentDownload">Download the files without listing everything</param>
        public static void Download (this RenciSftpClient client, SiteCredentials credentials,
            MappedPath dir, SftpFilter filter, Boolean replace, Boolean silentDownload = false) {
            var files = SftpUtils.ListFiles (client, dir.GetFullRemotePath (), filter, silentDownload);
            string fileName, serverCopy;
            FileInfo cC, wC;
            AuraSftpClient.SSHTransactionVoid (credentials, (Action<AuraSftpClient>) ((AuraSftpClient c) => {
                foreach (String remoteFile in files) {
                    fileName = remoteFile.Replace ((string) dir.GetFullRemotePath (), "").Substring (1).Replace ("/", "\\");
                    serverCopy = Path.Combine ((string) dir.GetFullServerCopy (), fileName);
                    //Cache copy
                    cC = new FileInfo (serverCopy);
                    //The path to the working copy
                    wC = new FileInfo (Path.Combine ((string) dir.GetFullProjectCopy (), fileName));
                    DownloadFile (c, remoteFile, cC, wC, replace, silentDownload);
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
                sC = new FileInfo (fileMap.GetFullServerCopy ());
                lC = new FileInfo (fileMap.GetFullProjectCopy ());
                DownloadFile (c, fileMap.GetFullRemotePath (), sC, lC, replace);
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
        /// <param name="serverCopy">The server copy path, also called the cache copy. Defines where cache file is stored</param>
        /// <param name="localCopy">The project copy path, also called the working copy. Defines where the editable file is stored.</param>
        /// <param name="replace">True if the project copy file is replaced if exists</param>
        /// <param name="silentDownload">Download the files without listing details</param>
        private static void DownloadFile (AuraSftpClient client, string remoteFile,
            FileInfo serverCopy, FileInfo localCopy, Boolean replace, Boolean silentDownload = false) {
            if (!silentDownload)
                Console.WriteLine (String.Format (MSG_INF_DOW_FILE, remoteFile));

            if (!Directory.Exists (serverCopy.Directory.FullName))
                Directory.CreateDirectory (serverCopy.Directory.FullName);
            client.Download (remoteFile, serverCopy);

            if (!Directory.Exists (localCopy.Directory.FullName))
                Directory.CreateDirectory (localCopy.Directory.FullName);
            if (File.Exists (localCopy.FullName) && !replace && !silentDownload)
                Console.WriteLine (String.Format (MSG_INF_EXIST_OMIT_FILE, localCopy.FullName));
            else {
                if (replace || !File.Exists (localCopy.FullName))
                    File.Copy (serverCopy.FullName, localCopy.FullName, replace);
                if (!silentDownload) {
                    if (!File.Exists (localCopy.FullName))
                        Console.WriteLine (String.Format (MSG_INF_COPY_FILE, localCopy.FullName));
                    else
                        Console.WriteLine (String.Format (MSG_INF_EXIST_REPLACE_FILE, localCopy.FullName));
                }
            }
            //Cache is always replaced
            string copyPath = serverCopy.FullName + ".copy";
            if (File.Exists (copyPath))
                File.Delete (copyPath);
            File.Move (serverCopy.FullName, copyPath);
        }
        /// <summary>
        /// Uploads a collection of files to the current server site
        /// </summary>
        /// <param name="files">The collection of files to upload</param>
        /// <param name="project">The active project</param>
        /// <param name="silentMode">if true push changes without confirmation</param>
        public static void UploadFiles (IEnumerable<MappedPath> files, Project project, Boolean silentMode = false) {
            Console.WriteLine (MSG_TIT_FILES_UP);
            files.ToList ().ForEach (x => Console.WriteLine (x.ToUploadPreviewFormat ()));
            if (silentMode || CommandUtils.AskYesNo (MSG_ASK_CONTINUE)) {
                AuraSftpClient.SFTPTransactionVoid (project.Connection.Data, (SftpClient c) => {
                    var uploadedFiles = files.OrderBy (x => x.GetFullRemotePath ()).ToArray ();
                    Stream input;
                    foreach (MappedPath file in uploadedFiles) {
                        input = File.OpenRead (file.GetFullProjectCopy ());
                        using (input = File.OpenRead (file.GetFullProjectCopy ())) {
                            c.UploadFile (input, file.GetFullRemotePath (), true, null);
                            Console.WriteLine (String.Format ("Uploaded at {0}", file.GetFullRemotePath ()));
                            File.Copy (file.GetFullProjectCopy (), file.GetFullServerCopy () + ".copy", true);
                        }
                    }
                });
            }
        }
    }
}