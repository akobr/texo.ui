using System;
using System.Collections.Generic;

namespace BeaverSoft.Texo.Commands.FileManager.Path
{
    public static class PathExtensions
    {
        private static char[] InvalidPathCharacters =
        {
            '\"', '<', '>', '|', '\0',
            (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10,
            (char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
            (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
            (char)31
        };

        private static char[] ExtraInvalidPathCharacters =
        {
            '\"', '<', '>', '|', '\0',
            (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10,
            (char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
            (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
            (char)31, '*', '?'
        };

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
            return character == WildcardPath.WILDCARD_ANY_CHARACTER
                || character == WildcardPath.WILDCARD_ONE_CHARACTER;
        }

        private static bool IsValidDriveChar(char value)
        {
            return (value >= 'A' && value <= 'Z') || (value >= 'a' && value <= 'z');
        }
    }
}
