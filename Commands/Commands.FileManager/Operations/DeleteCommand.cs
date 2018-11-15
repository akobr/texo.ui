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
    public class DeleteCommand : ICommand
    {
        private readonly IStageService stage;

        public DeleteCommand(IStageService stage)
        {
            this.stage = stage ?? throw new ArgumentNullException(nameof(stage));
        }

        public ICommandResult Execute(CommandContext context)
        {
            if (!stage.TryGetPaths(out var paths))
            {
                return new TextResult("The stage is empty.");
            }

            return Delete(new DeleteContext
            {
                Items = paths,
                SourceLobby = stage.GetLobby(),
                Preview = context.HasOption(ApplyOptions.PREVIEW)
            });
        }

        private static ICommandResult Delete(DeleteContext context)
        {
            var result = ImmutableList<Item>.Empty.ToBuilder();

            foreach (string path in context.Items)
            {
                switch (path.GetPathType())
                {
                    case PathTypeEnum.File:
                        result.Add(DeleteFile(path, context));
                        break;

                    case PathTypeEnum.Directory:
                        result.Add(DeleteDirectory(path, context));
                        break;

                    // case PathTypeEnum.NonExistent:
                    default:
                        result.Add(ItemBuilding.BuildMissingItem(path.GetFriendlyPath(context.SourceLobby)));
                        break;
                }
            }

            return new ItemsResult(result.ToImmutable());
        }

        private static Item DeleteFile(string filePath, DeleteContext context)
        {
            ItemDeleteContext fileContext = PrepareFile(filePath, context);

            if (!context.Preview)
            {
                ProcessFile(fileContext);
            }

            return BuildFileItem(fileContext);
        }

        private static ItemDeleteContext PrepareFile(string filePath, DeleteContext context)
        {
            return new ItemDeleteContext
            {
                Path = filePath,
                Source = filePath.GetFriendlyPath(context.SourceLobby),
                Descriptiton = context.Preview
                    ? "File will be deleted."
                    : "File has been deleted."
            };
        }

        private static void ProcessFile(ItemDeleteContext fileContext)
        {
            try
            {
                File.Delete(fileContext.Path);
            }
            catch (Exception exception)
            {
                fileContext.Descriptiton = exception.Message;
            }
        }

        private static Item BuildFileItem(ItemDeleteContext fileContext)
        {
            MarkdownBuilder builder = new MarkdownBuilder();
            builder.Header(fileContext.Source);
            builder.Italic(fileContext.Descriptiton);
            return Item.Markdown(builder.ToString());
        }

        private static Item DeleteDirectory(string directoryPath, DeleteContext context)
        {
            ItemDeleteContext directoryContext = PrepareDirectory(directoryPath, context);

            if (!context.Preview)
            {
                ProcessDirectory(directoryContext);
            }

            return BuildDirectoryItem(directoryContext);
        }

        private static ItemDeleteContext PrepareDirectory(string directoryPath, DeleteContext context)
        {
            return new ItemDeleteContext
            {
                Path = directoryPath,
                Source = directoryPath.GetFriendlyPath(context.SourceLobby),
                Descriptiton = context.Preview
                    ? "Directory will be deleted."
                    : "Directory has been deleted."
            };
        }

        private static void ProcessDirectory(ItemDeleteContext directoryContext)
        {
            try
            {
                Directory.Delete(directoryContext.Path, true);
            }
            catch (Exception exception)
            {
                directoryContext.Descriptiton = exception.Message;
            }
        }
        private static Item BuildDirectoryItem(ItemDeleteContext directoryContext)
        {
            MarkdownBuilder builder = new MarkdownBuilder();
            builder.Header(directoryContext.Source);
            builder.Write(directoryContext.Descriptiton);
            return Item.Markdown(builder.ToString());
        }

        private class DeleteContext
        {
            public string SourceLobby;
            public IEnumerable<string> Items;
            public bool Preview;
        }

        private class ItemDeleteContext
        {
            public string Path;
            public string Source;
            public string Descriptiton;
        }
    }
}
