using System;
using System.Collections.Generic;
using System.IO;
using BeaverSoft.Texo.Commands.FileManager.Extensions;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Commands.FileManager.Stash;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.Path;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Commands.FileManager.Operations
{
    public class DeleteCommand : ICommand
    {
        private readonly IStageService stage;
        private readonly IStashService stashes;
        private readonly ILogService logger;

        public DeleteCommand(IStageService stage, IStashService stashes, ILogService logger)
        {
            this.stage = stage ?? throw new ArgumentNullException(nameof(stage));
            this.stashes = stashes ?? throw new ArgumentNullException(nameof(stashes));
            this.logger = logger;
        }

        public ICommandResult Execute(CommandContext context)
        {
            IOperationSource source = context.GetOperationSource(stage, stashes);

            if (source.IsNullOrEmpty())
            {
                return new TextResult("The stage is empty.");
            }

            return Delete(new DeleteContext
            {
                Items = source.GetPaths(),
                SourceLobby = source.GetLobby(),
                Preview = context.HasOption(ApplyOptions.PREVIEW)
            });
        }

        private ICommandResult Delete(DeleteContext context)
        {
            foreach (string path in context.Items)
            {
                switch (path.GetPathType())
                {
                    case PathTypeEnum.File:
                        DeleteFile(path, context);
                        break;

                    case PathTypeEnum.Directory:
                        DeleteDirectory(path, context);
                        break;
                }
            }

            return new ItemsResult(Item.AsMarkdown(BuildOutput(context)));
        }

        private void DeleteFile(string filePath, DeleteContext context)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            context.DeletedFiles.Add(filePath.GetFriendlyPath(context.SourceLobby));
            ProcessFileDelete(filePath, context);
        }

        private void ProcessFileDelete(string filePath, DeleteContext context)
        {
            if (context.Preview)
            {
                return;
            }

            try
            {
                File.Delete(filePath);
            }
            catch (Exception exception)
            {
                logger.Error("File delete: " + filePath, exception);
            }
        }

        private void DeleteDirectory(string directoryPath, DeleteContext context)
        {
            foreach (string filePath in TexoDirectory.GetFiles(directoryPath))
            {
                DeleteFile(filePath, context);
            }

            if (context.Preview
               || !TexoDirectory.IsEmpty(directoryPath))
            {
                return;
            }

            context.DeletedFolders.Add(directoryPath.GetFriendlyPath(context.SourceLobby));
            ProcessDirectoryDelete(directoryPath);
        }

        private void ProcessDirectoryDelete(string directoryPath)
        {
            try
            {
                Directory.Delete(directoryPath, true);
            }
            catch (Exception exception)
            {
                logger.Error("Directory delete: " + directoryPath, exception);
            }
        }

        private static string BuildOutput(DeleteContext context)
        {
            MarkdownBuilder builder = new MarkdownBuilder();
            bool empty = true;

            if (context.DeletedFiles.Count > 0)
            {
                builder.Header("Deleted files");
                builder.WriteRawPathList(context.DeletedFiles);
                empty = false;
            }

            if (context.DeletedFolders.Count > 0)
            {
                builder.Header("Deleted directories");
                builder.WriteRawPathList(context.DeletedFolders);
                empty = false;
            }

            if (empty)
            {
                builder.Italic("Nothing deleted.");
            }

            return builder.ToString();
        }

        private class DeleteContext
        {
            public string SourceLobby;
            public IEnumerable<string> Items;
            public bool Preview;

            public readonly List<string> DeletedFiles = new List<string>();
            public readonly List<string> DeletedFolders = new List<string>();
        }
    }
}
