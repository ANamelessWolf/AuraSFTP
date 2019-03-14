using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nameless.Libraries.Aura.Utils;

namespace Nameless.Libraries.Aura.Model {

    /// <summary>
    /// This class manage a mapped path
    /// </summary>
    public class RelativeMappedPath : MappedPath {

        /// <summary>
        /// Gets the remote root path
        /// </summary>
        private string RemoteRootPath;
        /// <summary>
        /// Gets the local root path
        /// </summary>
        private string LocalRootPath;
        /// <summary>
        /// Initialize a new instance of the relative mapped class
        /// </summary>
        /// <param name="remoteRootPath">The remote root path</param>
        /// <param name="localRootPath">The local root path </param>
        public RelativeMappedPath (String remoteRootPath, String localRootPath) {
            this.RelativePath = true;
            this.RemoteRootPath = remoteRootPath;
            this.LocalRootPath = localRootPath;
        }
        /// <summary>
        /// Gets the remote absolute path
        /// </summary>
        public override string GetFullRemotePath () {
            return MappingUtils.GetPath (this.RemoteRootPath, new String[] { this.RemotePath }, false);
        }
        /// <summary>
        /// Gets the server copy absolute path
        /// </summary>
        public override string GetFullProjectCopy () {
            return MappingUtils.GetPath (this.LocalRootPath, new String[] { this.ProjectCopy });
        }
        /// <summary>
        /// Gets the project absolute copy path
        /// </summary>
        public override string GetFullServerCopy () {
            return MappingUtils.GetPath (this.LocalRootPath, new String[] { this.ServerCopy });
        }
        /// <summary>
        /// Gets the format
        /// </summary>
        /// <returns>The Upload format preview</returns>
        public override String ToUploadPreviewFormat () {
            String format = "{0} -->> {1}";
            return String.Format (format, this.GetFullProjectCopy (), this.GetFullRemotePath ());
        }
    }
}