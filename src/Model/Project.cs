using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Nameless.Libraries.Aura.Model {
    /// <summary>
    /// This class is used to deserialize a project configuration file
    /// </summary>
    public class Project {
        /// <summary>
        /// Defines the connection datta
        /// </summary>
        public SiteDefinition Connection;
        /// <summary>
        /// Defines the project configuration data
        /// </summary>
        public ProjectDefinition Data;
        /// <summary>
        /// Creates a filter using the current project
        /// </summary>
        /// <returns>The Sftp filter</returns>
        [IgnoreDataMemberAttribute]
        public SftpFilter Filter { get => new SftpFilter (this); }
    }
}