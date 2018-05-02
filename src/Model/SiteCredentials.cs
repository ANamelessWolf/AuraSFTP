using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nameless.Libraries.Aura.Model
{
    /// <summary>
    /// This class is used to deserialize the site SFTP credentials
    /// </summary>
    public class SiteCredentials
    {
        /// <summary>
        /// Defines the SFTP host
        /// </summary>
        public String  Host;
        /// <summary>
        /// Defines the SFTP username
        /// </summary>
        public String  User;
        /// <summary>
        /// Defines the SFTP password
        /// </summary>
        public String  Password;
        /// <summary>
        /// Defines the host SFTP start directory
        /// </summary>
        public String  RootDir;
        /// <summary>
        /// Defines the SFTP port
        /// </summary>
        public int  Port;
    }
}