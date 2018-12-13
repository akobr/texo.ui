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
    public class MoveCommand : ICommand
    {
        private readonly IStageService stage;

        public MoveCommand(IStageService stage)
        {
            this.stage = stage ?? throw new ArgumentNullException(nameof(stage));
        }

        public ICommandResult Execute(CommandContext context)
        {
            if (!stage.TryGetPaths(out var paths))
            {
                return new TextResult("The stage is empty.");
            }

            MoveContext moveContext = new MoveContext
            {
                Items = paths,
                Destination = context.GetTargetDirectory(),
                SourceLobby = stage.GetLobby(),
                Flat = context.HasOption(ApplyOptions.FLATTEN) || !stage.HasLobby(),
                Override = context.HasOption(ApplyOptions.OVERRIDE),
                Preview = context.HasOption(ApplyOptions.PREVIEW)
            };

            return Move(moveContext);
        }

        private static ICommandResult Move(MoveContext context)
        {
            var result = ImmutableList<Item>.Empty.ToBuilder();

            foreach (string path in context.Items)
            {
                switch (path.GetPathType())
                {
                    case PathTypeEnum.File:
                        result.Add(CopyFile(path, context));
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

        private static Item CopyFile(string filePath, MoveContext context)
        {
            FileMoveContext fileContext = PrepareFileCopy(filePath, context);
            ProcessFile(fileContext, context);
            return BuildFileItem(fileContext, context);
        }

        private static Item CopyDirectory(string directoryPath, MoveContext context)
        {
            DirectoryMoveContext directoryContext = PrepareDirectoryCopy(directoryPath, context);
            ProcessDirectory(directoryContext, context);
            return BuildDirectoryItem(directoryContext, context);
        }

        private static void ProcessFile(FileMoveContext fileContext, MoveContext context)
        {
            if (context.Preview)
            {
                return;
            }

            try
            {
                if (fileContext.Overriden)
                {
                    File.Delete(fileContext.Destination);
                }

                File.Move(fileContext.Source, fileContext.Destination);
            }
            catch (Exception exception)
            {
                fileContext.Exception = exception;
            }
        }

        private static void ProcessDirectory(DirectoryMoveContext directoryContext, MoveContext context)
        {
            if (context.Preview)
            {
                return;
            }

            foreach (FileMoveContext fileContext in directoryContext.Files)
            {
                ProcessFile(fileContext, context);
            }

            // TODO: Delete folder?
        }

        private static Item BuildFileItem(FileMoveContext fileContext, MoveContext context)
        {
            MarkdownBuilder builder = new MarkdownBuilder();
            WriteFileItem(fileContext, context, builder, 1);
            return Item.Markdown(builder.ToString());
        }


        private static Item BuildDirectoryItem(DirectoryMoveContext directoryContext, MoveContext context)
        {
            MarkdownBuilder builder = new MarkdownBuilder();
            builder.Header(directoryContext.Source.GetFriendlyPath(context.SourceLobby));
            builder.Write(directoryContext.Destination);

            foreach (FileMoveContext fileContext in directoryContext.Files)
            {
                WriteFileItem(fileContext, context, builder, 2);
            }

            return Item.Markdown(builder.ToString());
        }

        private static FileMoveContext PrepareFileCopy(string filePath, MoveContext context)
        {
            bool flatten = context.Flat || !filePath.IsSubPathOf(context.SourceLobby);
            string destinationPath = context.Destination.CombinePathWith(
                flatten
                    ? filePath.GetFileNameOrDirectoryName()
                    : filePath.GetRelativePath(context.SourceLobby));

            return new FileMoveContext
            {
                Source = filePath,
                Destination = destinationPath,
                Overriden = File.Exists(destinationPath)
            };
        }

        private static DirectoryMoveContext PrepareDirectoryCopy(string directoryPath, MoveContext context)
        {
            bool flatten = context.Flat || !directoryPath.IsSubPathOf(context.SourceLobby);
            string destinationPath = context.Destination.CombinePathWith(
                flatten
                    ? directoryPath.GetFileNameOrDirectoryName()
                    : directoryPath.GetRelativePath(context.SourceLobby));

            List<FileMoveContext> files = new List<FileMoveContext>();
            foreach (string filePath in TexoDirectory.GetFiles(directoryPath))
            {
                files.Add(PrepareFileCopy(filePath, context));
            }

            return new DirectoryMoveContext
            {
                Source = directoryPath,
                Destination = destinationPath,
                Files = files
            };
        }

        private static void WriteFileItem(FileMoveContext fileContext, MoveContext context, MarkdownBuilder builder, int headerLevel)
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


        private class MoveContext
        {
            public string Destination;
            public string SourceLobby;
            public IEnumerable<string> Items;
            public bool Override;
            public bool Flat;
            public bool Preview;
        }

        private class FileMoveContext
        {
            public string Source;
            public string Destination;
            public bool Overriden;
            public Exception Exception;
        }

        private class DirectoryMoveContext
        {
            public string Source;
            public string Destination;
            public List<FileMoveContext> Files;
        }
    }
}
