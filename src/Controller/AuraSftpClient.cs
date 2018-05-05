using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nameless.Libraries.Aura.Model;
using Renci.SshNet;
using RenCiSftpClient = Renci.SshNet.SftpClient;
using static Renci.SshNet.RemotePathTransformation;
using Nameless.Libraries.Aura.Utils;

namespace Nameless.Libraries.Aura.Controller {

    public class AuraSftpClient : ScpClient {
        /// <summary>
        /// The Sftp connection credentials
        /// </summary>
        SiteCredentials credentials;

        /// <summary>
        /// Start the connection to SFTP client
        /// </summary>
        /// <param name="_cred">The SFTP connection credentials</param>
        public AuraSftpClient (SiteCredentials _cred) : base (_cred.Host, _cred.Port, _cred.User, _cred.Password) {
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
        public static Object SSHTransaction (SiteCredentials _cred, Func<AuraSftpClient, Object> task, IRemotePathTransformation remotePath = null) {
            Object result = null;
            using (var client = new AuraSftpClient (_cred)) {
                try {
                    if (remotePath == null)
                        client.RemotePathTransformation = ShellQuote;
                    else
                        client.RemotePathTransformation = remotePath;
                    client.Connect ();
                    result = task (client);
                    client.Disconnect ();
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
        public static T SSHTransactionGen<T> (SiteCredentials _cred, Func<AuraSftpClient, T> task, IRemotePathTransformation remotePath = null)
        where T : class => (T) SSHTransaction (_cred, task);
        /// <summary>
        /// Defines a SSH transaction
        /// </summary>
        /// <param name="_cred">The SSH transaction credentials</param>
        /// <param name="task">The Transaction task</param>
        /// <param name="remotePath">Changes how the remote path is transformed before 
        /// it is passed to the scp command on the remote server.
        /// DoubleQuote, ShellQuote, None</param>
        public static void SSHTransactionVoid (SiteCredentials _cred, Action<AuraSftpClient> task, IRemotePathTransformation remotePath = null) {
            using (var client = new AuraSftpClient (_cred)) {
                try {
                    if (remotePath == null)
                        client.RemotePathTransformation = ShellQuote;
                    else
                        client.RemotePathTransformation = remotePath;
                    client.Connect ();
                    task (client);
                    client.Disconnect ();
                } catch (System.Exception exc) {
                    Console.WriteLine (exc.Message);
                }
            }
        }
        /// <summary>
        /// Defines a SFTP Client transaction
        /// </summary>
        /// <param name="_cred">The SSH transaction credentials</param>
        /// <param name="task">The Transaction task</param>
        /// <returns>The transaction result</returns>
        public static Object SFTPTransaction (SiteCredentials _cred, Func<Renci.SshNet.SftpClient, Object> task) {
            Object result = null;
            using (var client = new Renci.SshNet.SftpClient (_cred.Host, _cred.Port, _cred.User, _cred.Password)) {
                try {
                    client.Connect ();
                    result = task (client);
                    client.Disconnect ();
                } catch (System.Exception exc) {
                    Console.WriteLine (exc.Message);
                }
            }
            return result;
        }
        /// <summary>
        /// Defines a SFTP Client transaction
        /// </summary>
        /// <param name="_cred">The SSH transaction credentials</param>
        /// <param name="task">The Transaction task</param>
        /// <returns>The transaction result</returns>
        public static T SFTPTransactionGen<T> (SiteCredentials _cred, Func<Renci.SshNet.SftpClient, T> task)
        where T : class => (T) SFTPTransaction (_cred, task);
        /// <summary>
        /// Defines a SFTP Client transaction
        /// </summary>
        /// <param name="_cred">The SSH transaction credentials</param>
        /// <param name="task">The Transaction task</param>
        public static void SFTPTransactionVoid (SiteCredentials _cred, Action<Renci.SshNet.SftpClient> task) {
            using (var client = new Renci.SshNet.SftpClient (_cred.Host, _cred.Port, _cred.User, _cred.Password)) {
                try {
                    client.Connect ();
                    task (client);
                    client.Disconnect ();
                } catch (System.Exception exc) {
                    Console.WriteLine (exc.Message);
                }
            }
        }

    }
}