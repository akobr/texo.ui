using System;
using System.Collections.Generic;
using System.IO;
using BeaverSoft.Texo.Commands.FileManager.Extensions;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Commands.FileManager.Stash;
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
        private readonly IStashService stashes;
        private readonly ILogService logger;

        public CopyCommand(IStageService stage, IStashService stashes, ILogService logger)
        {
            this.stage = stage ?? throw new ArgumentNullException(nameof(stage));
            this.stashes = stashes ?? throw new ArgumentNullException(nameof(stashes));
            this.logger = logger;
        }

        public ICommandResult Execute(CommandContext context)
        {
            var paths = context.GetPaths(stage, stashes);

            if (paths.Count < 1)
            {
                return new TextResult("The stage is empty.");
            }

            CopyContext copyContext = new CopyContext
            {
                Items = paths,
                Destination = context.GetTargetDirectory(),
                SourceLobby = stage.GetLobby(),
                Flat = context.HasOption(ApplyOptions.FLATTEN) || !stage.HasLobby(),
                Overwrite = context.HasOption(ApplyOptions.OVERWRITE),
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

            return new ItemsResult(Item.Markdown(BuildOutput(context)));
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
                if (!context.Overwrite)
                {
                    return;
                }

                context.OverwritenFiles.Add(destinationPath);
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
                File.Copy(filePath, destinationPath, context.Overwrite);

            }
            catch (Exception exception)
            {
                logger.Error("File copy: " + filePath, destinationPath, exception);
            }
        }

        private static string BuildOutput(CopyContext context)
        {
            MarkdownBuilder builder = new MarkdownBuilder();
            builder.Header("Copy");
            builder.Link(context.Destination, ActionBuilder.PathOpenUri(context.Destination));
            bool empty = true;

            if (context.CopiedFiles.Count > 0)
            {
                builder.Header("Copied", 2);
                builder.WritePathList(context.CopiedFiles, context.Destination);
                empty = false;
            }

            if (context.OverwritenFiles.Count > 0)
            {
                builder.Header("Overwriten");
                builder.WritePathList(context.OverwritenFiles, context.Destination);
                empty = false;
            }

            if (empty)
            {
                builder.WriteLine();
                builder.WriteLine();
                builder.Italic("Nothing copied.");
            }

            return builder.ToString();
        }

        private class CopyContext
        {
            public string Destination;
            public string SourceLobby;
            public IEnumerable<string> Items;
            public bool Overwrite;
            public bool Flat;
            public bool Preview;

            public readonly IList<string> CopiedFiles = new List<string>();
            public readonly IList<string> OverwritenFiles = new List<string>();
        }
    }
}