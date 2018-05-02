using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nameless.Libraries.Aura.Model {

    /// <summary>
    /// This class manage a mapped path
    /// </summary>
    public class MappedPath {
        /// <summary>
        /// Gets the remote file path
        /// </summary>
        public string RemotePath;
        /// <summary>
        /// Gets the server copy path
        /// </summary>
        public string ServerCopy;
        /// <summary>
        /// Gets the project current copy
        /// </summary>
        public string ProjectCopy;
    }
}