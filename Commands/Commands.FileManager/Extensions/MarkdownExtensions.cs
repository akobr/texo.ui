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
        public static void WriteExistingPathList(this MarkdownBuilder builder, IEnumerable<string> paths, string relatedTo)
        {
            LinksModel model = BuildLinks(paths, relatedTo);
            WritePaths(model.Directories, builder);
            WritePaths(model.Files, builder);
        }

        public static void WritePathLists(this MarkdownBuilder builder, IEnumerable<string> paths, string relatedTo)
        {
            LinksModel model = BuildLinks(paths, relatedTo);

            WritePathsWithHeader(model.Directories, "Directories", builder);
            WritePathsWithHeader(model.Files, "Files", builder);
            WritePathsWithHeader(model.NonExists, "Non-Existing", builder);
        }

        private static void WritePathsWithHeader(List<ILink> paths, string title, MarkdownBuilder builder)
        {
            if (paths.Count <= 0)
            {
                return;
            }

            builder.Header(title, 2);
            builder.WriteLine();

            WritePaths(paths, builder);
        }

        private static void WritePaths(List<ILink> paths, MarkdownBuilder builder)
        {
            if (paths.Count <= 0)
            {
                return;
            }

            paths.Sort((first, second) =>
               string.Compare(first.Title, second.Title, StringComparison.OrdinalIgnoreCase));

            foreach (ILink directory in paths)
            {
                builder.Bullet();
                builder.Link(directory);
            }
        }

        private static LinksModel BuildLinks(IEnumerable<string> paths, string relatedTo)
        {
            LinksModel result = new LinksModel();

            foreach (string path in paths)
            {
                PathTypeEnum type = path.GetPathType();

                switch (type)
                {
                    case PathTypeEnum.Directory:
                        result.Directories.Add(new Link(path.GetFriendlyPath(relatedTo), ActionBuilder.DirectoryOpenUri(path)));
                        break;

                    case PathTypeEnum.File:
                        result.Files.Add(new Link(path.GetFriendlyPath(relatedTo), ActionBuilder.FileOpenUri(path)));
                        break;

                    case PathTypeEnum.NonExistent:
                        result.NonExists.Add(new Link(path.GetFriendlyPath(relatedTo), ActionBuilder.PathUri(path)));
                        break;

                }
            }

            return result;
        }

        private class LinksModel
        {
            public readonly List<ILink> Directories = new List<ILink>();
            public readonly List<ILink> Files = new List<ILink>();
            public readonly List<ILink> NonExists = new List<ILink>();
        }
    }
}
