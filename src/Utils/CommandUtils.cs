using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nameless.Libraries.Aura.Model;
using Newtonsoft.Json;

namespace Nameless.Libraries.Aura.Utils {

    public static class CommandUtils {
        /// <summary>
        /// Splits and array into two arrays using a given index
        /// </summary>
        /// <param name="array">The array to split</param>
        /// <param name="index">The array index</param>
        /// <param name="first">The first array</param>
        /// <param name="second">The second array</param>
        public static void Split<T> (this T[] array, int index, out T[] first, out T[] second) {
            first = array.Take (index).ToArray ();
            second = array.Skip (index).ToArray ();
        }
    }
}