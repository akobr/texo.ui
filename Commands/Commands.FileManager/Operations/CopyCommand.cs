using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.Path;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Commands.FileManager.Operations
{
    public class CopyCommand : ICommand
    {
        private readonly IStageService stage;

        public CopyCommand(IStageService stage)
        {
            this.stage = stage ?? throw new ArgumentNullException(nameof(stage));
        }

        public ICommandResult Execute(CommandContext context)
        {
            if (!stage.TryGetPaths(out var paths))
            {
                return new TextResult("The stage is empty.");
            }

            CopyContext copyContext = new CopyContext
            {
                Items = paths,
                Destination = context.GetTargetDirectory(),
                SourceLobby = stage.GetLobby(),
                Flat = context.HasOption(ApplyOptions.FLATTEN) || !stage.HasLobby(),
                Override = context.HasOption(ApplyOptions.OVERRIDE),
                Preview = context.HasOption(ApplyOptions.PREVIEW)
            };

            return Copy(copyContext);
        }

        private static ICommandResult Copy(CopyContext context)
        {
            IMarkdownBuilder builder = new MarkdownBuilder();
            builder.Header("Copy");

            foreach (string path in context.Items)
            {
                switch (path.GetPathType())
                {
                    case PathTypeEnum.File:
                        CopyFile(path, context);
                        break;

                    case PathTypeEnum.Directory:
                        result.Add(CopyDirectory(path, context));
                        break;

                    // case PathTypeEnum.NonExistent:
                    default:
                        result.Add(ItemBuilding.BuildMissingItem(path.GetFriendlyPath(context.SourceLobby)));
                        break;
                }
            }

            return new ItemsResult(result.ToImmutable());
        }

        private static Item CopyFile(string filePath, CopyContext context)
        {
            FileCopyContext fileContext = PrepareFileCopy(filePath, context);
            ProcessFile(fileContext, context);
            return BuildFileItem(fileContext, context);
        }

        private static Item CopyDirectory(string directoryPath, CopyContext context)
        {
            DirectoryCopyContext directoryContext = PrepareDirectoryCopy(directoryPath, context);
            ProcessDirectory(directoryContext, context);
            return BuildDirectoryItem(directoryContext, context);
        }

        private static void ProcessFile(FileCopyContext fileContext, CopyContext context)
        {
            if (context.Preview)
            {
                return;
            }

            try
            {
                string directory = fileContext.Destination.GetParentDirectoryPath();
                Directory.CreateDirectory(directory);
                File.Copy(fileContext.Source, fileContext.Destination, fileContext.Overriden);
            }
            catch (Exception exception)
            {
                fileContext.Exception = exception;
            }
        }

        private static void ProcessDirectory(DirectoryCopyContext directoryContext, CopyContext context)
        {
            if (context.Preview)
            {
                return;
            }

            foreach (FileCopyContext fileContext in directoryContext.Files)
            {
                ProcessFile(fileContext, context);
            }
        }

        private static Item BuildFileItem(FileCopyContext fileContext, CopyContext context)
        {
            MarkdownBuilder builder = new MarkdownBuilder();
            WriteFileItem(fileContext, context, builder, 1);
            return Item.Markdown(builder.ToString());
        }


        private static Item BuildDirectoryItem(DirectoryCopyContext directoryContext, CopyContext context)
        {
            MarkdownBuilder builder = new MarkdownBuilder();
            builder.Header(directoryContext.Source.GetFriendlyPath(context.SourceLobby));
            builder.Write(directoryContext.Destination);

            foreach (FileCopyContext fileContext in directoryContext.Files)
            {
                WriteFileItem(fileContext, context, builder, 2);
            }

            return Item.Markdown(builder.ToString());
        }

        private static FileCopyContext PrepareFileCopy(string filePath, CopyContext context)
        {
            bool flatten = context.Flat || !filePath.IsSubPathOf(context.SourceLobby);
            string destinationPath = context.Destination.CombinePathWith(
                flatten
                    ? filePath.GetFileNameOrDirectoryName()
                    : filePath.GetRelativePath(context.SourceLobby));

            return new FileCopyContext()
            {
                Source = filePath,
                Destination = destinationPath,
                Overriden = File.Exists(destinationPath)
            };
        }

        private static DirectoryCopyContext PrepareDirectoryCopy(string directoryPath, CopyContext context)
        {
            bool flatten = context.Flat || !directoryPath.IsSubPathOf(context.SourceLobby);
            string destinationPath = context.Destination.CombinePathWith(
                flatten
                    ? directoryPath.GetFileNameOrDirectoryName()
                    : directoryPath.GetRelativePath(context.SourceLobby));

            List<FileCopyContext> files = new List<FileCopyContext>();
            foreach (string filePath in TexoDirectory.GetFiles(directoryPath))
            {
                files.Add(PrepareFileCopy(filePath, context));
            }

            return new DirectoryCopyContext
            {
                Source = directoryPath,
                Destination = destinationPath,
                Files = files
            };
        }

        private static void WriteFileItem(FileCopyContext fileContext, CopyContext context, MarkdownBuilder builder, int headerLevel)
        {
            string header = fileContext.Source.GetFriendlyPath(context.SourceLobby);
            string description;
            string destination = fileContext.Destination;

            if (fileContext.Exception != null)
            {
                description = fileContext.Exception.Message;
            }
            else if (fileContext.Overriden)
            {
                if (context.Override)
                {
                    description = context.Preview
                        ? "File will be copy and a destination file overridden."
                        : "File has been copied and the destination file overridden.";
                }
                else
                {
                    description = context.Preview
                        ? "File won't be copy, the destination file already exists."
                        : "File hasn't been copied, the destination file already exists.";
                }
            }
            else
            {
                description = context.Preview
                    ? "File will be copy."
                    : "File has been copied.";
            }

            builder.Header(header, headerLevel);
            builder.Italic(description);
            builder.WriteLine();
            builder.Write(destination);
        }


        private class CopyContext
        {
            public string Destination;
            public string SourceLobby;
            public IEnumerable<string> Items;
            public bool Override;
            public bool Flat;
            public bool Preview;

            public IMarkdownBuilder DirectoryResult;
            public IMarkdownBuilder FileResult;
            public IMarkdownBuilder MissingResult;
        }

        private class FileCopyContext
        {
            public string Source;
            public string Destination;
            public bool Overriden;
            public Exception Exception;
        }

        private class DirectoryCopyContext
        {
            public string Source;
            public string Destination;
            public List<FileCopyContext> Files;
        }
    }
}