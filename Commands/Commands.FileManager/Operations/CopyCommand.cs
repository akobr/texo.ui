using System;
using System.Collections.Generic;
using System.IO;
using BeaverSoft.Texo.Commands.FileManager.Extensions;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.Path;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Commands.FileManager.Operations
{
    public class CopyCommand : ICommand
    {
        private readonly IStageService stage;
        private readonly ILogService logger;

        public CopyCommand(IStageService stage, ILogService logger)
        {
            this.stage = stage ?? throw new ArgumentNullException(nameof(stage));
            this.logger = logger;
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
                Override = context.HasOption(ApplyOptions.OVERWRITE),
                Preview = context.HasOption(ApplyOptions.PREVIEW)
            };

            return Copy(copyContext);
        }

        private ICommandResult Copy(CopyContext context)
        {
            foreach (string path in context.Items)
            {
                switch (path.GetPathType())
                {
                    case PathTypeEnum.File:
                        CopyFile(path, context);
                        break;

                    case PathTypeEnum.Directory:
                        CopyDirectory(path, context);
                        break;
                }
            }

            MarkdownBuilder builder = new MarkdownBuilder();
            builder.Header("Copy");
            builder.Link(context.Destination, ActionBuilder.DirectoryOpenUri(context.Destination));
            builder.WriteExistingPathList(context.CopiedFiles, context.Destination);
            return new ItemsResult(Item.Markdown(builder.ToString()));
        }

        private void CopyFile(string filePath, CopyContext context)
        {
            bool flatten = context.Flat || !filePath.IsSubPathOf(context.SourceLobby);
            string destinationPath = context.Destination.CombinePathWith(
            flatten
                ? filePath.GetFileNameOrDirectoryName()
                : filePath.GetRelativePath(context.SourceLobby));

            if (File.Exists(destinationPath))
            {
                if (!context.Override)
                {
                    return;
                }

                context.OverridenFiles.Add(destinationPath);
            }
            else
            {
                context.CopiedFiles.Add(destinationPath);
            }

            ProcessFileCopy(filePath, destinationPath, context);
        }

        private void CopyDirectory(string directoryPath, CopyContext context)
        {
            foreach (string filePath in TexoDirectory.GetFiles(directoryPath))
            {
                CopyFile(filePath, context);
            }
        }

        private void ProcessFileCopy(string filePath, string destinationPath, CopyContext context)
        {
            if (context.Preview)
            {
                return;
            }

            try
            {
                string directory = destinationPath.GetParentDirectoryPath();
                Directory.CreateDirectory(directory);
                File.Copy(filePath, destinationPath, context.Override);

            }
            catch (Exception exception)
            {
                logger.Error("File copy: " + filePath, exception);
            }
        }

        private class CopyContext
        {
            public string Destination;
            public string SourceLobby;
            public IEnumerable<string> Items;
            public bool Override;
            public bool Flat;
            public bool Preview;

            public readonly IList<string> CopiedFiles = new List<string>();
            public readonly IList<string> OverridenFiles = new List<string>();
        }
    }
}