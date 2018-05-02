using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nameless.Libraries.Aura.Model {
    /// <summary>
    /// This class is used to deserialize the a site definition
    /// </summary>
    public class SiteDefinition {
        /// <summary>
        /// Defines the site name,
        /// An alias to identify the site
        /// </summary>
        public String Site;
        /// <summary>
        /// Defines the SFTP Site data
        /// </summary>
        public SiteCredentials Data;
    }
}