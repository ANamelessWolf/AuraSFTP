using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nameless.Libraries.Aura.Model {
    /// <summary>
    /// This class represents the project definition
    /// </summary>
    public class ProjectDefinition 
    { 
        /// <summary>
        /// The name of the project
        /// </summary>
        public String Name;
        /// <summary>
        /// The path that stores the last downloaded copy of the server
        /// </summary>
        public String ServerCopy;
        /// <summary>
        /// The path that stores the project editable files
        /// </summary>
        public String ProjectCopy;
        /// <summary>
        /// The last time the server copy was downloaded
        /// </summary>
        public DateTime ServerLastTimeCopy;
        /// <summary>
        /// The server local mapping
        /// </summary>
        public MapDefinition Map;
    }
}