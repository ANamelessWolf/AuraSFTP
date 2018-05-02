using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nameless.Libraries.Aura.data {

    public static class Message {
        /// <summary>
        /// The error message sent when no action is defined
        /// with the given arguments
        /// </summary>
        public const string MSG_ERR_BAD_ARGS = "No option was found with the given parameters.";
        /// <summary>
        /// The error message sent when no option is defined
        /// with the given action
        /// </summary>
        public const string MSG_ERR_BAD_CMD_OPTION = MSG_ERR_BAD_ARGS + " See '-h {0}' for more information.";
        /// <summary>
        /// The error message sent when no option is defined
        /// for the given command
        /// </summary>
        public const string MSG_ERR_BAD_OPTION = "No option was found for the {0} command. See '-h {1}' for more information.";
        /// <summary>
        /// The error message sent when no option is defined
        /// with the given arguments in the specified action.
        /// </summary>
        public const string MSG_ERR_BAD_CMD_ARGS = MSG_ERR_BAD_ARGS + " See '-h {0} {1}' for more information.";
        /*****************************
         ****** Project Messages ******
         *****************************/
        /// <summary>
        /// The error message sent when the specified path does not exists.
        /// </summary>
        public const string MSG_ERR_NEW_PRJ_MISS_DIR = "Can not create the project in the given directory. The directory does not exists";
        /// <summary>
        /// The error message sent when the project directory is not empty.
        /// </summary>
        public const string MSG_ERR_PRJ_NOT_EMPTY = "Can not create the project in the given directory. The directory is not empty";
        /// <summary>
        /// The error message sent when the project can not be open.
        /// </summary>
        public const string MSG_ERR_PRJ_OPEN = "An error occurred opening the project configuration file '%s'";
    }
}