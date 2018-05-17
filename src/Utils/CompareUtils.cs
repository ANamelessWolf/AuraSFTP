using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nameless.Libraries.Aura.Model;
using Nameless.Libraries.Aura.Utils.DiffMatchPatch;
using Newtonsoft.Json;
namespace Nameless.Libraries.Aura.Utils {

    public static class CompareUtils {
        /// <summary>
        /// Compares two files to see its difference
        /// </summary>
        /// <param name="projectFilePath">The project copy file path</param>
        /// <param name="dmp">The diff match path</param>
        /// <param name="serverFilePath">The server copy file path</param>
        /// <returns>The file differences</returns>
        public static List<Diff> CompareFile (this String projectFilePath, diff_match_patch dmp, String serverFilePath) {
            String projectFile = String.Join ("\r\n", File.ReadAllLines (projectFilePath)),
                serverFile = String.Join ("\r\n", File.ReadAllLines (serverFilePath));
            return dmp.diff_main ( serverFile,projectFile);
        }
    }
}