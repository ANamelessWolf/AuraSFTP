using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nameless.Libraries.Aura.Controller;
using Nameless.Libraries.Aura.Model;
using static Nameless.Libraries.Aura.Resources .Message;
using Newtonsoft.Json;

namespace Nameless.Libraries.Aura.Utils {
    /// <summary>
    /// This class implements the project manipulation tasks
    /// </summary>
    public static class ProjectUtils {
        /// <summary>
        /// Initialize a new project
        /// </summary>
        /// <param name="prjName">The name of the project</param>
        /// <param name="prjPath">The project path</param>
        /// <returns>The project configuration class</returns>
        public static Project InitProject (String prjName, String prjPath) {
            String prjDir = Path.Combine (prjPath, prjName);
            String prjSettDir = Path.Combine (prjDir, ".ssh");
            String prjTmp = Path.Combine (SiteUtils.GetBinPath(), "data", "project_template.json");
            String config = Path.Combine (prjSettDir, "config.json");
            //1: Create Project Directory
            if (!Directory.Exists (prjDir))
                Directory.CreateDirectory (prjDir);
            //2: Create Project Settings Folder, .ssh folder is hidden
            DirectoryInfo sshDir = Directory.CreateDirectory (prjSettDir);
            sshDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            //3: Create Server copy
            DirectoryInfo serverCp = Directory.CreateDirectory (Path.Combine (prjSettDir, "ServerCopy"));
            //4: Add the configuration file
            File.Copy (prjTmp, config);
            Project prj = OpenProjectFile (config);
            prj.Connection = ConsoleMenuController.Connection;
            prj.Data.Name = prjName;
            prj.Data.ProjectCopy = prjDir;
            prj.Data.ServerCopy = serverCp.FullName;
            prj.Data.ServerLastTimeCopy = DateTime.Now;
            SaveProject (prj, config);
            return prj;
        }
        /// <summary>
        /// Saves the current project configuration file in the given path
        /// </summary>
        /// <param name="prj">The project configuration file</param>
        /// <param name="pth">The project configuration file path</param>
        public static void SaveProject (this Project prj, string pth) {
            File.WriteAllText (pth, JsonConvert.SerializeObject (prj));
        }

        /// <summary>
        /// Opens a project configuration file
        /// </summary>
        /// <param name="configPth">The configuration file path</param>
        /// <returns>The open project</returns>
        public static Project OpenProjectFile (this string configPth) {
            if (File.Exists (configPth)) {
                using (StreamReader r = new StreamReader (configPth)) {
                    string json = r.ReadToEnd ();
                    return JsonConvert.DeserializeObject<Project> (json);
                }
            } else
                throw new Exception (String.Format (MSG_ERR_PRJ_OPEN, configPth));
        }
        /// <summary>
        /// Opens the current project
        /// </summary>
        /// <throw>An exception is thrown if the current directory doesn't has a project</throw>
        /// <returns>The current project</returns>
        public static Project OpenProject () {
            String localPath = Environment.CurrentDirectory;
            String configFile = Path.Combine (localPath, ".ssh", "config.json");
            Boolean sshProjectExists = File.Exists (configFile);
            if (sshProjectExists)
                return configFile.OpenProjectFile ();
            else
                throw new Exception (MSG_ERR_PRJ_MISS);
        }
    }
}