using System;
using System.Collections.Generic;
using System.IO;

namespace BeaverSoft.Texo.Core.Path
{
    public static class PathExtensions
    {
        private const string TEXO_FOLDER_NAME = "Texo";

        private static readonly char[] InvalidPathCharacters =
        {
            '\"', '<', '>', '|', '\0',
            (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10,
            (char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
            (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
            (char)31
        };

        private static readonly char[] ExtraInvalidPathCharacters =
        {
            '\"', '<', '>', '|', '\0',
            (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10,
            (char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
            (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
            (char)31, '*', '?'
        };

        public static string GetTexoDataFolder()
        {
            string appDataFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            return System.IO.Path.Combine(appDataFolder, TEXO_FOLDER_NAME);
        }

        public static string GetRelativePath(this string path)
        {
            return GetRelativePath(path, System.Environment.CurrentDirectory);
        }

        public static string GetRelativePath(this string path, string relativeTo)
        {
            if (string.IsNullOrWhiteSpace(path)
                || string.IsNullOrWhiteSpace(relativeTo))
            {
                return string.Empty;
            }

            Uri pathUri = new Uri(path);

            if (!IsDirectorySeparator(relativeTo[relativeTo.Length - 1]))
            {
                relativeTo += System.IO.Path.DirectorySeparatorChar;
            }

            Uri folderUri = new Uri(relativeTo);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString());
        }

        public static PathTypeEnum GetPathType(this string path)
        {
            if (File.Exists(path))
            {
                return PathTypeEnum.File;
            }

            if (Directory.Exists(path))
            {
                return PathTypeEnum.Directory;
            }

            return PathTypeEnum.NonExistent;
        }

        public static IEnumerable<string> GetDirectories(this IEnumerable<string> paths)
        {
            if (paths == null)
            {
                yield break;
            }

            foreach (string path in paths)
            {
                if (Directory.Exists(path))
                {
                    yield return path;
                }
            }
        }

        public static IEnumerable<string> GetFiles(this IEnumerable<string> paths)
        {
            if (paths == null)
            {
                yield break;
            }

            foreach (string path in paths)
            {
                if (File.Exists(path))
                {
                    yield return path;
                }
            }
        }

        public static IEnumerable<string> GetNonExistents(this IEnumerable<string> paths)
        {
            if (paths == null)
            {
                yield break;
            }

            foreach (string path in paths)
            {
                if (!File.Exists(path) && !Directory.Exists(path))
                {
                    yield return path;
                }
            }
        }

        public static bool IsValidWildcardPath(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            return path.IndexOfAny(InvalidPathCharacters) < 0;
        }

        public static bool IsValidPath(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            return path.IndexOfAny(ExtraInvalidPathCharacters) < 0;
        }

        public static bool IsRelative(this string path)
        {
            if (path == null
                || IsDirectorySeparator(path[0]))
            {
                return false;
            }

            if (path.Length >= 3
                && path[1] == System.IO.Path.VolumeSeparatorChar
                && IsDirectorySeparator(path[2])
                && IsValidDriveChar(path[0]))
            {
                return false;
            }

            return true;
        }

        public static IEnumerable<string> SplitToSegments(this string path)
        {
            if (path == null)
            {
                yield break;
            }

            string[] rawSegments = path.Split(new[]
            {
                System.IO.Path.DirectorySeparatorChar,
                System.IO.Path.AltDirectorySeparatorChar
            }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string rawSegment in rawSegments)
            {
                if(string.IsNullOrWhiteSpace(rawSegment))
                {
                    continue;
                }

                yield return rawSegment;
            }
        }

        public static bool IsDirectorySeparator(this char character)
        {
            return character == System.IO.Path.DirectorySeparatorChar
                || character == System.IO.Path.AltDirectorySeparatorChar;
        }

        public static bool IsWildcard(this char character)
        {
            return character == TexoPath.WILDCARD_ANY_CHARACTER
                || character == TexoPath.WILDCARD_ONE_CHARACTER;
        }

        private static bool IsValidDriveChar(char value)
        {
            return (value >= 'A' && value <= 'Z') || (value >= 'a' && value <= 'z');
        }
    }
}
