using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BeaverSoft.Texo.Core.Path
{
    // TODO: solve cycles in NTFS file system
    // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-iterate-through-a-directory-tree
    public static class TexoDirectory
    {
        public static bool IsEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        public static IEnumerable<string> GetDirectories()
        {
            return GetDirectories(
                System.Environment.CurrentDirectory,
                PathConstants.SEARCH_TERM_ALL,
                SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> GetDirectories(string path)
        {
            return GetDirectories(path, PathConstants.SEARCH_TERM_ALL, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> GetDirectories(string path, string searchPattern)
        {
            return GetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> GetDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            if (!Directory.Exists(path))
            {
                yield break;
            }

            Stack<string> directories = new Stack<string>();
            directories.Push(path);

            while(directories.Count > 0)
            {
                string directory = directories.Pop();
                string[] subDirectories;

                try
                {
                    subDirectories = Directory.GetDirectories(directory, searchPattern);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                catch (DirectoryNotFoundException)
                {
                    continue;
                }

                foreach (string subDirectory in subDirectories)
                {
                    yield return subDirectory;

                    if (searchOption == SearchOption.AllDirectories)
                    {
                        directories.Push(subDirectory);
                    }
                }
            }
        }

        public static IEnumerable<string> GetFiles()
        {
            return GetFiles(
                System.Environment.CurrentDirectory,
                PathConstants.SEARCH_TERM_ALL,
                SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> GetFiles(string path)
        {
            return GetFiles(path, PathConstants.SEARCH_TERM_ALL, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> GetFiles(string path, string searchPattern)
        {
            return GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            if (!Directory.Exists(path))
            {
                yield break;
            }

            Stack<string> directories = new Stack<string>();
            directories.Push(path);

            while (directories.Count > 0)
            {
                string directory = directories.Pop();
                string[] files;

                try
                {
                    files = Directory.GetFiles(directory, searchPattern);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                catch (DirectoryNotFoundException)
                {
                    continue;
                }

                foreach (string file in files)
                {
                    yield return file;
                }

                if (searchOption != SearchOption.AllDirectories)
                {
                    continue;
                }

                string[] subDirectories;

                try
                {
                    subDirectories = Directory.GetDirectories(directory, searchPattern);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                catch (DirectoryNotFoundException)
                {
                    continue;
                }

                foreach (string subDirectory in subDirectories)
                {
                    directories.Push(subDirectory);
                }
            }
        }

        public static IEnumerable<string> GetFileSystemEntries()
        {
            return GetFileSystemEntries(
                System.Environment.CurrentDirectory,
                PathConstants.SEARCH_TERM_ALL,
                SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> GetFileSystemEntries(string path)
        {
            return GetFileSystemEntries(path, PathConstants.SEARCH_TERM_ALL, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> GetFileSystemEntries(string path, string searchPattern)
        {
            return GetFileSystemEntries(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> GetFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
        {
            if (!Directory.Exists(path))
            {
                yield break;
            }

            Stack<string> directories = new Stack<string>();
            directories.Push(path);

            while (directories.Count > 0)
            {
                string directory = directories.Pop();
                string[] subDirectories;

                try
                {
                    subDirectories = Directory.GetDirectories(directory, searchPattern);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                catch (DirectoryNotFoundException)
                {
                    continue;
                }

                foreach (string subDirectory in subDirectories)
                {
                    yield return subDirectory;

                    if (searchOption == SearchOption.AllDirectories)
                    {
                        directories.Push(subDirectory);
                    }
                }

                subDirectories = null;
                string[] files;

                try
                {
                    files = Directory.GetFiles(directory, searchPattern);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                catch (DirectoryNotFoundException)
                {
                    continue;
                }

                foreach (string file in files)
                {
                    yield return file;
                }
            }
        }

        public static IEnumerable<string> GetFileSystemEntries(string path, string searchPattern, string firstLevelPattern)
        {
            if (!Directory.Exists(path))
            {
                yield break;
            }

            Stack<string> directories = new Stack<string>();
            string[] firstDirectories;

            try
            {
                firstDirectories = Directory.GetDirectories(path, firstLevelPattern);
            }
            catch (UnauthorizedAccessException)
            {
                yield break;
            }
            catch (DirectoryNotFoundException)
            {
                yield break;
            }

            foreach (string firstDirectory in firstDirectories)
            {
                yield return firstDirectory;
                directories.Push(firstDirectory);
            }

            string[] firstFiles;

            try
            {
                firstFiles = Directory.GetFiles(path, firstLevelPattern);
            }
            catch (UnauthorizedAccessException)
            {
                firstFiles = new string[0];
            }
            catch (DirectoryNotFoundException)
            {
                firstFiles = new string[0];
            }

            foreach (string file in firstFiles)
            {
                yield return file;
            }

            while (directories.Count > 0)
            {
                string directory = directories.Pop();
                string[] subDirectories;

                try
                {
                    subDirectories = Directory.GetDirectories(directory, searchPattern);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                catch (DirectoryNotFoundException)
                {
                    continue;
                }

                foreach (string subDirectory in subDirectories)
                {
                    yield return subDirectory;
                    directories.Push(subDirectory);
                }

                subDirectories = null;
                string[] files;

                try
                {
                    files = Directory.GetFiles(directory, searchPattern);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                catch (DirectoryNotFoundException)
                {
                    continue;
                }

                foreach (string file in files)
                {
                    yield return file;
                }
            }
        }
    }
}
