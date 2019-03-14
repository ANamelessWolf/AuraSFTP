using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Nameless.Libraries.Aura.Utils.DiffMatchPatch {

    /// <summary>
    /// Class containing the diff, match and patch methods.
    /// Also Contains the behavior settings.
    /// </summary>
    public partial class diff_match_patch {
        /// <summary>
        /// Set these on your diff_match_patch instance to override the defaults. 
        /// Number of seconds to map a diff before giving up (0 for infinity).
        /// </summary>
        public float Diff_Timeout = 1.0f;
        /// <summary>
        /// Cost of an empty edit operation in terms of edit characters.
        /// </summary>
        public short Diff_EditCost = 4;
        /// <summary>
        /// At what point is no match declared (0.0 = perfection, 1.0 = very loose).
        /// </summary>
        public float Match_Threshold = 0.5f;
        /// <summary>
        /// How far to search for a match (0 = exact location, 1000+ = broad match).
        /// A match this many characters away from the expected location will add
        /// 1.0 to the score (0.0 is a perfect match).
        /// </summary>
        public int Match_Distance = 1000;
        /// <summary>
        /// When deleting a large block of text (over ~64 characters), how close
        /// do the contents have to be to match the expected contents. (0.0 =
        /// perfection, 1.0 = very loose).  Note that Match_Threshold controls
        /// how closely the end points of a delete need to match.
        /// </summary>
        public float Patch_DeleteThreshold = 0.5f;
        /// <summary>
        /// Chunk size for context length.
        /// </summary>
        public short Patch_Margin = 4;

        /// <summary>
        /// The number of bits in an int
        /// </summary>
        private short Match_MaxBits = 32;

        /// <summary>
        /// Define some regex patterns for matching boundaries at the
        /// end of the line.
        /// </summary>
        /// <returns>The regular expression</returns>
        private Regex BLANKLINEEND => new Regex ("\\n\\r?\\n\\Z");
        /// <summary>
        /// Define some regex patterns for matching boundaries at the start
        /// of the line.
        /// </summary>
        /// <returns>The regular expression</returns>
        private Regex BLANKLINESTART => new Regex ("\\A\\r?\\n\\r?\\n");
        /// <summary>
        /// Define some regex patterns for HEXCODE
        /// </summary>
        /// <returns>The regular expression</returns>
        private static Regex HEXCODE => new Regex ("%[0-9A-F][0-9A-F]");
        /// <summary>
        /// Unescape selected chars for compatibility with JavaScript's encodeURI.
        /// In speed critical applications this could be dropped since the
        /// receiving application will certainly decode these fine.
        /// Note that this function is case-sensitive.  Thus "%3F" would not be
        /// unescaped.  But this is ok because it is only called with the output of
        /// Uri.EscapeDataString which returns lowercase hex.
        /// </summary>
        /// <example>"%3f" -> "?", "%24" -> "$", etc.</example>
        /// <param name="str">The string to escape</param>
        /// <returns>The escaped string</returns>
        public static string unescapeForEncodeUriCompatability (string str) {
            str = str.Replace ("%20", " ").Replace ("%21", "!").Replace ("%2A", "*")
                .Replace ("%27", "'").Replace ("%28", "(").Replace ("%29", ")")
                .Replace ("%3B", ";").Replace ("%2F", "/").Replace ("%3F", "?")
                .Replace ("%3A", ":").Replace ("%40", "@").Replace ("%26", "&")
                .Replace ("%3D", "=").Replace ("%2B", "+").Replace ("%24", "$")
                .Replace ("%2C", ",").Replace ("%23", "#");
            return HEXCODE.Replace (str, new MatchEvaluator (lowerHex));
        }

        private static string lowerHex (Match m) {
            return m.ToString ().ToLower ();
        }
    }
}