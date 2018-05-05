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
        /// The error message sent when a command is excecuted outside a SSH
        /// </summary>
        public const string MSG_ERR_PRJ_MISS = "No project was found. Are you running the cmd in the root project directory?";
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
         ****** Project Messages *****
         *****************************/
        /// <summary>
        /// The message sent when the project is created.
        /// </summary>
        public const string MSG_INF_NEW_PRJ = "The SSH projected '{0}' was created correctly at the path '{1}'";
        /// <summary>
        /// The message sent when a file is download
        /// </summary>
        public const string MSG_INF_DOW_FILE = "Downloading the file '{0}'";
        /// <summary>
        /// The message sent when a file is copied
        /// </summary>
        public const string MSG_INF_COPY_FILE = "File copied at '{0}'";
        /// <summary>
        /// The message sent when a file is omitted because replace is deactivated
        /// </summary>
        public const string MSG_INF_EXIST_OMIT_FILE = "The file '{0}' already exists, replace is disable omitting.";
        /// <summary>
        /// The message sent when a file is replaced
        /// </summary>
        public const string MSG_INF_EXIST_REPLACE_FILE = "The file '{0}' already exists, replacing file.";
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
        public const string MSG_ERR_PRJ_OPEN = "An error occurred opening the project configuration file '{0}'";
        /// <summary>
        /// The error message sent when the project has nothing to pull.
        /// </summary>
        public const string MSG_ERR_PRJ_PULL_EMPTY_MAP = "There are no mapped directories or files. Nothing to download";

        /*****************************
         ****** Mapping Messages *****
         *****************************/
        /// <summary>
        /// The message sent when a mapping is added.
        /// </summary>
        public const string MSG_INF_MAP_CREATED = "A new mapped path from '{0}' to '{1}' was created correctly.";
        /// <summary>
        /// The error message sent when the remote path not exists
        /// </summary>
        public const string MSG_ERR_MAP_REM_PTH = "The remote path '{0}' was not found on the server.";
        /// <summary>
        /// The error message sent when the remote path is already mapped
        /// </summary>
        public const string MSG_ERR_MAP_AlREADY_MAPPED = "The remote path '{0}' is already mapped.";
        /// <summary>
        /// The error message sent when the remote path should be a directory
        /// </summary>
        public const string MSG_ERR_MAP_REM_PTH_NOT_DIR = "The remote path '{0}' is not a directory. See '-h {0} {1}' for more information.";
        /// <summary>
        /// The error message sent when the remote path should be a file
        /// </summary>
        public const string MSG_ERR_MAP_REM_PTH_NOT_FILE = "The remote path '{0}' is not a file. See '-h {0} {1}' for more information.";
    }
}