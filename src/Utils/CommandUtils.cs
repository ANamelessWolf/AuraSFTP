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
        /// <summary>
        /// Ask the user for a value
        /// </summary>
        /// <param name="msg">The query message</param>
        /// <returns>The value obtained from the user</returns>
        public static string Ask (string msg) {
            Console.WriteLine (msg);
            return Console.ReadLine ();
        }
        /// <summary>
        /// Ask the user for a password value
        /// </summary>
        /// <param name="msg">The query message</param>
        /// <returns>The value obtained from the user</returns>
        public static string AskPassword (string msg) {
            string pass = "";
            Console.Write (msg + "\n");
            ConsoleKeyInfo key;
            do {
                key = Console.ReadKey (true);
                if (key.Key != ConsoleKey.Backspace) {
                    pass += key.KeyChar;
                    Console.Write ("*");
                } else if (key.Key == ConsoleKey.Backspace && pass.Length > 0) {
                    pass = pass.Substring (0, (pass.Length - 1));
                    Console.Write ("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter); // Stops Receving Keys Once Enter is Pressed
            return pass;
        }
    }
}