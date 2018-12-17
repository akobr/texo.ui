using System;

namespace BeaverSoft.Texo.Core.Actions
{
    public static class ActionBuilder
    {
        private const string ACTION_SCHEMA = "action";
        private const string SCHEMA_SEPARATOR = "://";
        private const string ACTION_FORMAT = ACTION_SCHEMA + SCHEMA_SEPARATOR + "{0}?{1}";

        public static string PathUri(string fullPath)
        {
            return string.Format(ACTION_FORMAT, ActionNames.PATH, $"{ActionParameters.PATH}={Uri.EscapeDataString(fullPath)}");
        }

        public static string FileOpenUri(string fileFullPath)
        {
            return string.Format(ACTION_FORMAT, ActionNames.FILE_OPEN, $"{ActionParameters.PATH}={Uri.EscapeDataString(fileFullPath)}");
        }

        public static string DirectoryOpenUri(string directoryFullPath)
        {
            return string.Format(ACTION_FORMAT, ActionNames.DIRECTORY_OPEN, $"{ActionParameters.PATH}={Uri.EscapeDataString(directoryFullPath)}");
        }
    }
}
