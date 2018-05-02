using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nameless.Libraries.Aura.Model;
using Renci.SshNet;
using static Renci.SshNet.RemotePathTransformation;

namespace Nameless.Libraries.Aura.Controller {

    public class SftpClient : ScpClient {
        /// <summary>
        /// The Sftp connection credentials
        /// </summary>
        SiteCredentials credentials;

        /// <summary>
        /// Start the connection to SFTP client
        /// </summary>
        /// <param name="_cred">The SFTP connection credentials</param>
        public SftpClient (SiteCredentials _cred) : base (_cred.Host, _cred.Port, _cred.User, _cred.Password) {
            this.credentials = _cred;
        }
        /// <summary>
        /// Defines a SSH transaction
        /// </summary>
        /// <param name="_cred">The SSH transaction credentials</param>
        /// <param name="task">The Transaction task</param>
        /// <param name="remotePath">Changes how the remote path is transformed before 
        /// it is passed to the scp command on the remote server.
        /// DoubleQuote, ShellQuote, None</param>
        /// <returns>The transaction result</returns>
        public static Object SSHTransaction (SiteCredentials _cred, Func<SftpClient, Object> task, IRemotePathTransformation remotePath = null) {
            Object result = null;
            using (var client = new SftpClient (_cred)) {
                try {
                    if (remotePath == null)
                        client.RemotePathTransformation = ShellQuote;
                    else
                        client.RemotePathTransformation = remotePath;
                    client.Connect ();
                    result = task (client);
                } catch (System.Exception exc) {
                    Console.WriteLine (exc.Message);
                }
            }
            return result;
        }
        /// <summary>
        /// Defines a SSH transaction
        /// </summary>
        /// <param name="_cred">The SSH transaction credentials</param>
        /// <param name="task">The Transaction task</param>
        /// <param name="remotePath">Changes how the remote path is transformed before 
        /// it is passed to the scp command on the remote server.
        /// DoubleQuote, ShellQuote, None</param>
        /// <returns>The transaction result</returns>
        public static T SSHTransaction<T> (SiteCredentials _cred, Func<SftpClient, T> task, IRemotePathTransformation remotePath = null)
        where T : class => (T) SSHTransaction (_cred, task);
        /// <summary>
        /// Defines a SSH transaction
        /// </summary>
        /// <param name="_cred">The SSH transaction credentials</param>
        /// <param name="task">The Transaction task</param>
        /// <param name="remotePath">Changes how the remote path is transformed before 
        /// it is passed to the scp command on the remote server.
        /// DoubleQuote, ShellQuote, None</param>
        public static void SSHTransactionVoid (SiteCredentials _cred, Action<SftpClient> task, IRemotePathTransformation remotePath = null) {
            using (var client = new SftpClient (_cred)) {
                try {
                    if (remotePath == null)
                        client.RemotePathTransformation = ShellQuote;
                    else
                        client.RemotePathTransformation = remotePath;
                    client.Connect ();
                    task (client);
                } catch (System.Exception exc) {
                    Console.WriteLine (exc.Message);
                }
            }
        }
        /// <summary>
        /// List all the files in the given directory
        /// </summary>
        /// <param name="dirPath">The directory path</param>
        /// <returns>The files inside the directory</returns>
        public IEnumerable<String> ListFiles (string dirPath) {
            IEnumerable<String> result = new String[0];
            using (var client = new Renci.SshNet.SftpClient (this.credentials.Host, this.credentials.Port,
                this.credentials.User, this.credentials.Password)) {
                client.Connect ();
                result = this.ListFiles (client, dirPath);
                client.Disconnect ();
            }
            return result;
        }
        /// <summary>
        /// List the files inside a given directory
        /// </summary>
        /// <param name="client">The Sftp client</param>
        /// <param name="dirPath">The directory path</param>
        /// <returns>The file list collection</returns>
        private IEnumerable<String> ListFiles (Renci.SshNet.SftpClient client, string dirPath) {
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
    }
}