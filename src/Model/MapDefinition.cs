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
        /// Gets the mapped directories paths associated to the SSH project
        /// </summary>
        public MappedPath[] Directories;
        /// <summary>
        /// Gets the mapped files paths associated to the SSH project
        /// </summary>
        public MappedPath[] Files;
    }
}