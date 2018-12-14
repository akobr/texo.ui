using System;
using System.Collections.Generic;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.Path;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Commands.FileManager.Extensions
{
    public static class MarkdownExtensions
    {
        public static void WritePathLists(this MarkdownBuilder builder, string lobbyPath, IEnumerable<string> paths)
        {
            List<ILink> directories = new List<ILink>();
            List<ILink> files = new List<ILink>();
            List<ILink> nonExists = new List<ILink>();

            foreach (string path in paths)
            {
                PathTypeEnum type = path.GetPathType();

                switch (type)
                {
                    case PathTypeEnum.Directory:
                        directories.Add(new Link(path.GetFriendlyPath(lobbyPath), ActionBuilder.DirectoryOpenUri(path)));
                        break;

                    case PathTypeEnum.File:
                        files.Add(new Link(path.GetFriendlyPath(lobbyPath), ActionBuilder.FileOpenUri(path)));
                        break;

                    case PathTypeEnum.NonExistent:
                        nonExists.Add(new Link(path.GetFriendlyPath(lobbyPath), ActionBuilder.PathUri(path)));
                        break;

                }
            }

            WritePaths(directories, "Directories", builder);
            WritePaths(files, "Files", builder);
            WritePaths(nonExists, "Non-Existing", builder);
        }

        private static void WritePaths(List<ILink> paths, string title, MarkdownBuilder builder)
        {
            if (paths.Count <= 0)
            {
                return;
            }

            paths.Sort((first, second) =>
                string.Compare(first.Title, second.Title, StringComparison.OrdinalIgnoreCase));

            builder.Header(title, 2);
            builder.WriteLine();

            foreach (ILink directory in paths)
            {
                builder.Bullet();
                builder.Link(directory);
            }
        }
    }
}
