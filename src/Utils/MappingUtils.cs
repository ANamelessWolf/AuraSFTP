using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nameless.Libraries.Aura.Controller;
using Nameless.Libraries.Aura.Model;
using static Nameless.Libraries.Aura.Resources.Message;
using Newtonsoft.Json;

namespace Nameless.Libraries.Aura.Utils {
    /// <summary>
    /// This class implements the mapping manipulation tasks
    /// </summary>
    public static class MappingUtils {
        /// <summary>
        /// Gets the path combining a base path and an array of directories names
        /// </summary>
        /// <param name="basePath">The directory base path</param>
        /// <param name="dirNames">The directories names</param>
        /// <param name="winMode">If true windows directory separator is used otherwise linux separator</param>
        /// <returns>The Combined path</returns>
        public static String GetPath (String basePath, String[] dirNames, Boolean winMode = true) {
            Char search = winMode ? '/' : '\\',
                separator = winMode ? '\\' : '/';
            String pth = basePath;
            dirNames.ToList ().ForEach (x => pth = Path.Combine (pth, x));
            return pth.Replace (search, separator);
        }

        /// <summary>
        /// Validates and format a path, if the path is a directory should not end with a slash or
        /// back slash
        /// </summary>
        /// <param name="path">The entry path</param>
        /// <param name="winMode">If true windows directory separator is used otherwise linux separator</param>
        /// <returns>The validated path</returns>
        public static String ValidatePath (String path, Boolean winMode) {
            if (winMode)
                path = path.Replace ('/', '\\');
            else
                path = path.Replace ('\\', '/');
            if (path[path.Length - 1] == '/' || path[path.Length - 1] == '\\')
                path = path.Substring (0, path.Length - 1);
            return path;
        }
        /// <summary>
        /// Gets the mapped path
        /// </summary>
        /// <param name="prj">The active project</param>
        /// <param name="relativePath">The relative path to the mapped directory</param>
        /// <returns>The mapped path</returns>
        public static RelativeMappedPath GetMappedPath (Project prj, String relativePath) {
            String remotePath = ValidatePath (prj.Connection.Data.RootDir, false),
                localPath = ValidatePath (prj.Data.ProjectCopy, true);
            RelativeMappedPath pth = new RelativeMappedPath (remotePath, localPath);
            pth.RemotePath = relativePath;
            pth.ProjectCopy = relativePath.Replace ('/', '\\');
            pth.ServerCopy = Path.Combine (".ssh", "ServerCopy", relativePath.Replace ('/', '\\'));
            pth.LocalVersion = DateTime.Now;
            pth.RemoteVersion = DateTime.Now;
            return pth;
        }
        /// <summary>
        /// Gets the mapped path
        /// </summary>
        /// <param name="prj">The active project</param>
        /// <param name="localPath">The relative path to the local path in windows format</param>
        /// <param name="remotePath">The relative path to the remote path in linux format</param>
        /// <returns>The mapped path</returns>
        public static MappedPath GetMappedPath (Project prj, String localPath, String remotePath) {
            remotePath = ValidatePath (remotePath, false);
            localPath = ValidatePath (localPath, true);
            MappedPath pth = new MappedPath () {
                ProjectCopy = GetPath (ValidatePath (prj.Data.ProjectCopy, true), localPath.Split ('\\'), true),
                ServerCopy = GetPath (ValidatePath (prj.Data.ServerCopy, true), localPath.Split ('\\'), true),
                RemotePath = GetPath (ValidatePath (prj.Connection.Data.RootDir, false), remotePath.Split ('/'), false),
                RemoteVersion = DateTime.Now,
                LocalVersion = DateTime.Now
            };
            return pth;
        }
    }
}