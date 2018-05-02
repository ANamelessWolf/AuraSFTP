using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nameless.Libraries.Aura.Model {

    public class SSHProject {
        /// <summary>
        /// The name of the project
        /// </summary>
        public String Project;
        /// <summary>
        /// The path for the project data
        /// </summary>
        public String ProjectPath;
        /// <summary>
        /// The project mapped paths
        /// </summary>
        public MappedPath[] ProjectMap;
    }
}