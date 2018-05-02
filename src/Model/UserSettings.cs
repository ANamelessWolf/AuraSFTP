using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nameless.Libraries.Aura.Model {

    /// <summary>
    /// This class is used to deserialize the application user settings
    /// </summary>
    public class UserSettings {
        /// <summary>
        /// The index of the selected site
        /// </summary>
        public int SelectedSite;
        /// <summary>
        /// The list of sites defined by the user
        /// </summary>
        public SiteDefinition[] Sites;
    }
}