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
        /// Check if the file path contains a vali extension
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <param name="cExt">The extension to validate</param>
        /// <returns>True if the file is comparable</returns>
        public static Boolean IsComparable (this String filePath, String[] cExt) {
            FileInfo file = new FileInfo (filePath);
            return cExt.Select (x => "." + x.ToLower ()).FirstOrDefault (x => x == file.Extension.ToLower ()) != null;
        }

        /// <summary>
        /// Compares two files to see its difference
        /// </summary>
        /// <param name="dmp">The diff match path utility</param>
        /// <param name="projectFilePath">The project copy file path</param>
        /// <param name="serverFilePath">The server copy file path</param>
        /// <returns>The file differences</returns>
        public static List<Diff> CompareFiles (this diff_match_patch dmp, String projectFilePath, String serverFilePath) {
            String projectFile = String.Join ("\r\n", File.ReadAllLines (projectFilePath)),
                serverFile = String.Join ("\r\n", File.ReadAllLines (serverFilePath));
            return dmp.diff_main (serverFile, projectFile);
        }

        /// <summary>
        /// Compares two files to see if are equal
        /// </summary>
        /// <param name="dmp">The diff match path utility</param>
        /// <param name="projectFilePath">The project copy file path</param>
        /// <param name="serverFilePath">The server copy file path</param>
        /// <returns>True if the files are equals</returns>
        public static Boolean AreFilesEquals (this diff_match_patch dmp, String projectFilePath, String serverFilePath) {
            String projectFile = String.Join ("\r\n", File.ReadAllLines (projectFilePath)),
                serverFile = String.Join ("\r\n", File.ReadAllLines (serverFilePath));
            return dmp.diff_main (serverFile, projectFile).Count (x => x.operation != Operation.EQUAL) == 0;
        }
        /// <summary>
        /// Creates an html report
        /// </summary>
        /// <param name="dmp">The diff match path utility</param>
        /// <param name="baseFile">The file used as base to compare</param>
        /// <param name="compareFile">The file to compare with</param>
        /// <param name="baseFileHtmlResult">The diff founds in the base file as html encoding</param>
        /// <param name="compareFileHtmlResult">The diff founds in the compare file as html encoding</param>
        public static void CreateHtmlReport (this diff_match_patch dmp, FileInfo baseFile, FileInfo compareFile, out String baseFileHtmlResult, out String compareFileHtmlResult) {
            baseFileHtmlResult = dmp.diff_prettyHtml (dmp.CompareFiles (baseFile.FullName, compareFile.FullName)).Replace ("\r&para;", "");
            compareFileHtmlResult = dmp.diff_prettyHtml (dmp.CompareFiles (compareFile.FullName, baseFile.FullName)).Replace ("\r&para;", "");
        }

    }
}