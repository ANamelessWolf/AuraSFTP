using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nameless.Libraries.Aura.Resources {

    public static class Message {
        /*****************************
         **** Application Messages ***
         *****************************/
        /// <summary>
        /// The title used for uploading files
        /// </summary>
        public const string MSG_TIT_FILES_UP = "Files To upload:";
        /// <summary>
        /// The message used to ask for a value
        /// </summary>
        public const string MSG_ASK_CONTINUE = "Continue?";
        /// <summary>
        /// The message sent when the application configuration file was not found
        /// </summary>
        public const string MSG_ERR_APP_MISS_CONF = "Application 'settings.json' was not found.";
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
        /// The list of files that need to be uploaded to the server
        /// </summary>
        public const string MSG_INF_FILES_WITH_CHANGES = "The following files had changes from the server files";
        /// <summary>
        /// The message sent when a file is omitted because replace is deactivated
        /// </summary>
        public const string MSG_INF_EXIST_OMIT_FILE = "The file '{0}' already exists, replace is disable omitting.";
        /// <summary>
        /// The message sent when a file is replaced
        /// </summary>
        public const string MSG_INF_EXIST_REPLACE_FILE = "The file '{0}' already exists, replacing file.";
        /// <summary>
        /// The error message sent when the project has nothing to push.
        /// </summary>
        public const string MSG_INF_PRJ_PUSH_NO_CHANGES = MSG_INF_PRJ_NO_CHANGES + " Nothing to upload.";
        /// <summary>
        /// The error message sent when the project has nothing to push.
        /// </summary>
        public const string MSG_INF_PRJ_NO_CHANGES = "There are no changes in the project files.";
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
        /// The error message sent when the local path not exists.
        /// </summary>
        public const string MSG_ERR_PRJ_BAD_LOC_PTH = "The local path is not an existant file or an existant directory";
        /// <summary>
        /// The error message sent when the project has nothing to pull.
        /// </summary>
        public const string MSG_ERR_PRJ_PULL_EMPTY_MAP = "There are no mapped directories or files. Nothing to download";
        /// <summary>
        /// The error message sent when the project has nothing to push.
        /// </summary>
        public const string MSG_ERR_PRJ_PUSH_EMPTY_MAP = "There are no mapped directories or files. Nothing to upload. New files must be added first";
        /*****************************
         ****** Mapping Messages *****
         *****************************/
        /// <summary>
        /// The message sent when a mapping is added.
        /// </summary>
        public const string MSG_INF_MAP_CREATED = "A new mapped path from '{0}' to '{1}' was created correctly.";
        /// <summary>
        /// The message sent when a mapping is removed.
        /// </summary>
        public const string MSG_INF_MAP_REMOVED = "A mapped path '{0}' has been removed from the project.";
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
        public const string MSG_ERR_MAP_REM_PTH_NOT_DIR = "The remote path '{0}' is not a directory. See '-h {1} {2}' for more information.";
        /// <summary>
        /// The error message sent when the remote path should be a file
        /// </summary>
        public const string MSG_ERR_MAP_REM_PTH_NOT_FILE = "The remote path '{0}' is not a file. See '-h {1} {2}' for more information.";
        /*****************************
         ****** Site Messages ********
         *****************************/
        /// <summary>
        /// The message sent when a new site is added
        /// </summary>
        public const string MSG_INF_SITE_ADDED = "The site '{0}' has been added to the application.";
        /// <summary>
        /// The message sent when a site is modified
        /// </summary>
        public const string MSG_INF_SITE_UPDATED = "The site '{0}' information has been updated.";
        /// <summary>
        /// The message sent when a field from a site is modified
        /// </summary>
        public const string MSG_INF_SITE_FIELD_UPDATED = "The site '{0}' field '{1}' has been updated.";
        /// <summary>
        /// The message sent when a site is removed
        /// </summary>
        public const string MSG_INF_SITE_REMOVED = "The site '{0}' has been removed from the configuration file.";
        /// <summary>
        /// The message sent when a site connection succed
        /// </summary>
        public const string MSG_INF_SITE_CONN_SUCCED = "The site '{0}' has valid credentials to connect to the server.";
        /// <summary>
        /// The message sent when a site connection fails
        /// </summary>
        public const string MSG_INF_SITE_CONN_FAIL = "The site '{0}' could connect to the server, check your credentials. Server error: {1}";
        /// <summary>
        /// The message sent when a site is set as default
        /// </summary>
        public const string MSG_INF_SITE_DFTL = "The site '{0}' is set as default connection";
        /// <summary>
        /// The error message sent when the remote site was not found
        /// </summary>
        public const string MSG_ERR_SITE_NOT_FOUND = "The site configuration named '{0}' was not found.";
        /// <summary>
        /// The error message sent when no sites are defined
        /// </summary>
        public const string MSG_ERR_NO_SITES = "No sites defined run -s add.";
        /// <summary>
        /// The error message sent when the site exists
        /// </summary>
        public const string MSG_ERR_SITE_EXISTS = "The site '{0}' already exists use another name for the site";
        /// <summary>
        /// The error message sent when a site data is invalid
        /// </summary>
        public const string MSG_ERR_BAD_CRED = "The given credentials are not valid. Host, User and RootDir cannot be empty.";
        /// <summary>
        /// The error message sent when a site fail to be modified, because the given property not exists
        /// </summary>
        public const string MSG_ERR_SITE_UPDATED = "The site '{0}' doesn't has a property called '{1}'.";
        /****************************
         ****** Ignore Messages *****
         ****************************/
        /// <summary>
        /// The error message sent when an entry already exists
        /// </summary>
        public const string MSG_INF_IGNORE_ADDED = "Added extensions [{0}] to the '{1}' ignore list";
        /// <summary>
        /// The error message sent when an entry already exists
        /// </summary>
        public const string MSG_INF_IGNORE_ADD = "Added entry '{0}' to the '{1}' ignore list";
        /// <summary>
        /// The error message sent when an entry already exists
        /// </summary>
        public const string MSG_ERR_IGNORE_EXIST = "Already exists in ignore list";
        /// <summary>
        /// The error message sent when an extension doesn't start a period
        /// </summary>
        public const string MSG_ERR_BAD_EXT = "Extensions must start with a period";
        /// <summary>
        /// The error message sent when an entry is not defined in the ignore list
        /// </summary>
        public const string MSG_ERR_IGNORE_NOT_DEF = "The entry '{0}' was not found in the '{1}' list";
    }
}