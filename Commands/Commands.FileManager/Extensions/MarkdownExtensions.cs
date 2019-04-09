using System.Collections.Generic;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.Path;

namespace BeaverSoft.Texo.Commands.FileManager.Extensions
{
    public static class MarkdownExtensions
    {
        public static void WritePathLists(this MarkdownBuilder builder, string lobbyPath, IEnumerable<string> paths)
        {
            List<string> directories = new List<string>();
            List<string> files = new List<string>();
            List<string> nonExists = new List<string>();

            foreach (string path in paths)
            {
                PathTypeEnum type = path.GetPathType();

                switch (type)
                {
                    case PathTypeEnum.Directory:
                        directories.Add(path.GetFriendlyPath(lobbyPath));
                        break;

                    case PathTypeEnum.File:
                        files.Add(path.GetFriendlyPath(lobbyPath));
                        break;

                    case PathTypeEnum.NonExistent:
                        nonExists.Add(path.GetFriendlyPath(lobbyPath));
                        break;

                }
            }

            WritePaths(directories, "Directories", builder);
            WritePaths(files, "Files", builder);
            WritePaths(nonExists, "Non-Existing", builder);
        }

        private static void WritePaths(List<string> paths, string title, MarkdownBuilder builder)
        {
            if (paths.Count <= 0)
            {
                return;
            }

            paths.Sort();
            builder.Header(title, 2);
            builder.WriteLine();

            foreach (string directory in paths)
            {
                builder.Bullet(directory);
            }
        }
    }
}
