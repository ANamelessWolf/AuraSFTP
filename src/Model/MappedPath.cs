using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nameless.Libraries.Aura.Utils;
namespace Nameless.Libraries.Aura.Model {

    /// <summary>
    /// This class manage a mapped path
    /// </summary>
    public class MappedPath {
        /// <summary>
        /// Gets the flag value whether this is a relative or absolute path
        /// True if its a relative path
        /// </summary>
        public bool RelativePath;
        /// <summary>
        /// Gets the remote path
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
        /// <summary>
        /// Gets the local version date time
        /// </summary>
        public DateTime LocalVersion;
        /// <summary>
        /// Gets the remote version date time
        /// </summary>
        public DateTime RemoteVersion;
        /// <summary>
        /// Gets the format
        /// </summary>
        /// <returns>The Upload format preview</returns>
        public virtual String ToUploadPreviewFormat () {
            String format = "{0} -->> {1}";
            return String.Format (format, this.ProjectCopy, this.RemotePath);
        }
        /// <summary>
        /// Upgrades relatives mapped path
        /// </summary>
        /// <param name="prj">The active project</param>
        /// <returns>Upgrades the mapped path to a relative path</returns>
        public RelativeMappedPath UpgradeRelativeMappedPath (Project prj) {
            return MappingUtils.GetMappedPath (prj, this.RemotePath);
        }
        /// <summary>
        /// Gets the remote absolute path
        /// </summary>
        public virtual string GetFullRemotePath () {
            return MappingUtils.ValidatePath (this.RemotePath, false);
        }
        /// <summary>
        /// Gets the server copy absolute path
        /// </summary>
        public virtual string GetFullProjectCopy () {
            return MappingUtils.ValidatePath (this.ProjectCopy, true);
        }
        /// <summary>
        /// Gets the project absolute copy path
        /// </summary>
        public virtual string GetFullServerCopy () {
            return MappingUtils.ValidatePath (this.ServerCopy, true);
        }
    }
}