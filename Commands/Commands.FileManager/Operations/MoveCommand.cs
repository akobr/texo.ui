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
    public class MoveCommand : ICommand
    {
        private readonly IStageService stage;
        private readonly ILogService logger;

        public MoveCommand(IStageService stage, ILogService logger)
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

            MoveContext moveContext = new MoveContext
            {
                Items = paths,
                Destination = context.GetTargetDirectory(),
                SourceLobby = stage.GetLobby(),
                Flat = context.HasOption(ApplyOptions.FLATTEN) || !stage.HasLobby(),
                Overwrite = context.HasOption(ApplyOptions.OVERWRITE),
                Preview = context.HasOption(ApplyOptions.PREVIEW)
            };

            return Move(moveContext);
        }

        private ICommandResult Move(MoveContext context)
        {
            foreach (string path in context.Items)
            {
                switch (path.GetPathType())
                {
                    case PathTypeEnum.File:
                        MoveFile(path, context);
                        break;

                    case PathTypeEnum.Directory:
                        MoveDirectory(path, context);
                        break;
                }
            }

            return new ItemsResult(Item.Markdown(BuildOutput(context)));
        }

        private void MoveFile(string filePath, MoveContext context)
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
                context.MovedFiles.Add(destinationPath);
            }

            ProcessFileMove(filePath, destinationPath, context);
        }

        private void MoveDirectory(string directoryPath, MoveContext context)
        {
            foreach (string filePath in TexoDirectory.GetFiles(directoryPath))
            {
                MoveFile(filePath, context);
            }

            if (context.Preview
                || !TexoDirectory.IsEmpty(directoryPath))
            {
                return;
            }

            Directory.Delete(directoryPath);
        }


        private void ProcessFileMove(string filePath, string destinationPath, MoveContext context)
        {
            if (context.Preview)
            {
                return;
            }

            try
            {
                if (context.Overwrite)
                {
                    File.Delete(destinationPath);
                }

                string directory = destinationPath.GetParentDirectoryPath();
                Directory.CreateDirectory(directory);
                File.Move(filePath, destinationPath);
            }
            catch (Exception exception)
            {
                logger.Error("File move: " + filePath, destinationPath, exception);
            }
        }

        private static string BuildOutput(MoveContext context)
        {
            MarkdownBuilder builder = new MarkdownBuilder();
            builder.Header("Move");
            builder.Link(context.Destination, ActionBuilder.DirectoryOpenUri(context.Destination));
            bool empty = true;

            if (context.MovedFiles.Count > 0)
            {
                builder.Header("Moved", 2);
                builder.WritePathList(context.MovedFiles, context.Destination);
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
                builder.Italic("Nothing moved.");
            }

            return builder.ToString();
        }


        private class MoveContext
        {
            public string Destination;
            public string SourceLobby;
            public IEnumerable<string> Items;
            public bool Overwrite;
            public bool Flat;
            public bool Preview;

            public readonly IList<string> MovedFiles = new List<string>();
            public readonly IList<string> OverwritenFiles = new List<string>();
        }
    }
}
