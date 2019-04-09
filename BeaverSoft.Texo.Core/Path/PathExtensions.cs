using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace BeaverSoft.Texo.Core.Path
{
    public static class PathExtensions
    {
        public static string GetTexoDataDirectoryPath()
        {
            string appDataFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            return System.IO.Path.Combine(appDataFolder, PathConstants.TEXO_FOLDER_NAME);
        }

        public static string GetTexoDataDirectoryPath(string childDirectory)
        {
            return GetTexoDataDirectoryPath().CombinePathWith(childDirectory);
        }

        public static string GetAndCreateDataDirectoryPath(string childDirectory)
        {
            string fullPath = GetTexoDataDirectoryPath().CombinePathWith(childDirectory);
            Directory.CreateDirectory(fullPath);
            return fullPath;
        }

        public static string GetFullPath(this string path)
        {
            return System.IO.Path.GetFullPath(path);
        }

        public static string GetFullConsolidatedPath(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            return path.NormalisePath().GetFullPath().ToLowerInvariant();
        }

        public static bool IsSamePath(this string path, string otherPath)
        {
            if (!path.IsValidPath() || !otherPath.IsValidPath())
            {
                return false;
            }

            return string.Equals(
                path.GetFullPath(),
                otherPath.GetFullPath(),
                StringComparison.OrdinalIgnoreCase);
        }

        public static string GetFileNameOrDirectoryName(this string path)
        {
            return System.IO.Path.GetFileName(path);
        }

        public static string GetParentDirectoryPath(this string path)
        {
            return System.IO.Path.GetDirectoryName(path);
        }

        public static string CombinePathWith(this string path, string secondPath)
        {
            return System.IO.Path.Combine(path, secondPath);
        }

        public static string NormalisePath(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            return path.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
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

        public static string GetFriendlyPath(this string path)
        {
            return GetFriendlyPath(path, System.Environment.CurrentDirectory);
        }

        public static string GetFriendlyPath(this string path, string relativeTo)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(relativeTo)
                || !path.IsSubPathOf(relativeTo))
            {
                return path;
            }

            return GetRelativePath(path, relativeTo);
        }

        public static bool IsSubPathOf(this string childPath, string parentPath)
        {
            if (string.IsNullOrWhiteSpace(childPath)
                || string.IsNullOrWhiteSpace(parentPath))
            {
                return false;
            }

            childPath = NormalisePath(childPath);
            parentPath = NormalisePath(parentPath);

            if (childPath[childPath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
            {
                childPath += System.IO.Path.DirectorySeparatorChar;
            }

            if (parentPath[parentPath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
            {
                parentPath += System.IO.Path.DirectorySeparatorChar;
            }

            string childNormalisedFullPath = System.IO.Path.GetFullPath(childPath);
            string parentNormalisedFullPath = System.IO.Path.GetFullPath(parentPath);

            return childNormalisedFullPath.StartsWith(parentNormalisedFullPath, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsParentPathOf(this string parentPath, string childPath)
        {
            return IsSubPathOf(childPath, parentPath);
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

        public static IEnumerable<string> GetDirectoryPaths(this IEnumerable<string> paths)
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

        public static IEnumerable<string> GetFilePaths(this IEnumerable<string> paths)
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

        public static IEnumerable<string> GetAbsentPaths(this IEnumerable<string> paths)
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

        public static IEnumerable<string> SplitToPathSegments(this string path)
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
                if (string.IsNullOrWhiteSpace(rawSegment))
                {
                    continue;
                }

                yield return rawSegment;
            }
        }

        public static bool IsValidWildcardPath(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            return path.IndexOfAny(PathConstants.InvalidWildcardPathCharacters) < 0;
        }

        public static bool IsValidPath(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            return path.IndexOfAny(PathConstants.InvalidPathCharacters) < 0;
        }

        public static bool IsRelativePath(this string path)
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

        public static bool IsDriveRelativePath(this string path)
        {
            if (path == null
                || path.Length != 2)
            {
                return false;
            }

            return path[1] == System.IO.Path.VolumeSeparatorChar
                && IsValidDriveChar(path[0]);
        }

        public static bool IsDirectorySeparator(this char character)
        {
            return character == System.IO.Path.DirectorySeparatorChar
                || character == System.IO.Path.AltDirectorySeparatorChar;
        }

        public static bool IsPathWildcard(this char character)
        {
            return character == PathConstants.WILDCARD_ANY_CHARACTER
                || character == PathConstants.WILDCARD_ONE_CHARACTER;
        }

        public static bool IsVolumeSeparatorChar(this char value)
        {
            return value == System.IO.Path.VolumeSeparatorChar;
        }

        private static bool IsValidDriveChar(char value)
        {
            return (value >= 'A' && value <= 'Z') || (value >= 'a' && value <= 'z');
        }
    }
}
