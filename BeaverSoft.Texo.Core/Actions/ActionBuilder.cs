namespace BeaverSoft.Texo.Core.Actions
{
    public static class ActionBuilder
    {
        private const string ACTION_SCHEMA = "action";
        private const string SCHEMA_SEPARATOR = "://";
        private const string ACTION_FORMAT = ACTION_SCHEMA + SCHEMA_SEPARATOR + "{0}?{1}";

        public const string PATH = "path";
        public const string FILE = "file";
        public const string FILE_OPEN = "file-open";
        public const string DIRECTORY = "directory";
        public const string DIRECTORY_OPEN = "directory-open";

        public static string PathUri(string fullPath)
        {
            return string.Format(ACTION_FORMAT, PATH, $"path={fullPath}");
        }

        public static string FileOpenUri(string fileFullPath)
        {
            return string.Format(ACTION_FORMAT, FILE_OPEN, $"path={fileFullPath}");
        }

        public static string DirectoryOpenUri(string directoryFullPath)
        {
            return string.Format(ACTION_FORMAT, DIRECTORY_OPEN, $"path={directoryFullPath}");
        }
    }
}
