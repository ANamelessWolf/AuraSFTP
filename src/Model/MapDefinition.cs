using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nameless.Libraries.Aura.Model {
    /// <summary>
    /// This class is used to deserialize the server path mapping
    /// </summary>
    public class MapDefinition {
        /// <summary>
        /// Gets the mapped directories paths asociated to the SSH project
        /// </summary>
        public MappedPath[] Directories;
        /// <summary>
        /// Gets the mapped files paths asociated to the SSH project
        /// </summary>
        public MappedPath[] Files;
    }
}