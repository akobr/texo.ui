using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Commands.FileManager.Stash;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Model.Text;
using BeaverSoft.Texo.Core.Path;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Commands.FileManager.Operations
{
    public class ArchiveCommand : ICommand
    {
        private readonly IStageService stage;
        private readonly IStashService stashes;
        private readonly ILogService logger;

        public ArchiveCommand(IStageService stage, IStashService stashes, ILogService logger)
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

            return Zip(new ZipContext
            {
                Items = source.GetPaths(),
                DestinationZipFile = context.GetTargetFile(),
                SourceLobby = source.GetLobby(),
                Flat = context.HasOption(ApplyOptions.FLATTEN) || !source.HasLobby(),
                Override = context.HasOption(ApplyOptions.OVERWRITE),
                Add = context.HasOption(ApplyOptions.ADD)
            });
        }

        private ICommandResult Zip(ZipContext context)
        {
            context.ZipAlreadyExists = File.Exists(context.DestinationZipFile);

            if (context.ZipAlreadyExists
                && !context.Add
                && !context.Override)
            {
                return new ItemsResult(
                    Item.AsMarkdown("The zip file already exists, use options `--override` or `--add`."));
            }

            context.FileOutputList = new List();

            try
            {
                using (FileStream zipStream = new FileStream(context.DestinationZipFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Update))
                {
                    foreach (string path in context.Items)
                    {
                        switch (path.GetPathType())
                        {
                            case PathTypeEnum.File:
                                AddFile(path, archive, context);
                                break;

                            case PathTypeEnum.Directory:
                                AddDirectory(path, archive, context);
                                break;
                        }
                    }
                }

                if (context.FileOutputList.Items.Count < 1)
                {
                    context.FileOutputList = context.FileOutputList.AddItem(
                        new ListItem(new Italic("no new content added")));
                }

            }
            catch (Exception exception)
            {
                const string message = "Zip archive error.";
                logger.Error(message, exception);
                return new ErrorTextResult(message);
            }

            return new ItemsResult<ModeledItem>(BuildOutput(context));
        }

        private static void AddDirectory(string directoryPath, ZipArchive archive, ZipContext context)
        {
            foreach (string filePath in TexoDirectory.GetFiles(directoryPath))
            {
                AddFile(filePath, archive, context);
            }
        }

        private static void AddFile(string filePath, ZipArchive archive, ZipContext context)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            bool flatten = context.Flat || !filePath.IsSubPathOf(context.SourceLobby);
            string destinationPath = flatten
                ? filePath.GetFileNameOrDirectoryName()
                : filePath.GetFriendlyPath(context.SourceLobby);

            ZipArchiveEntry fileEntry = archive.GetEntry(destinationPath);

            if (fileEntry != null
                && !context.Override)
            {
                return;
            }

            if (fileEntry == null)
            {
                if (context.ZipAlreadyExists && !context.Add)
                {
                    return;
                }

                fileEntry = archive.CreateEntry(destinationPath);
            }

            context.FileOutputList = context.FileOutputList.AddItem(
                new ListItem(new Core.Model.Text.Link(
                    destinationPath,
                    ActionBuilder.PathOpenUri(filePath.GetFullPath()))));

            using (FileStream originalStream = new FileStream(filePath, FileMode.Open))
            using (Stream targetStream = fileEntry.Open())
            {
                originalStream.CopyTo(targetStream);
            }
        }

        private static ModeledItem BuildOutput(ZipContext context)
        {
            Document document = new Document(
                new Header(context.DestinationZipFile.GetFileNameOrDirectoryName()),
                new Paragraph(new Core.Model.Text.Link(
                    context.DestinationZipFile.GetFullPath(),
                    ActionBuilder.PathOpenUri(context.DestinationZipFile.GetFullPath()))),
                new Section(
                    new Header(2, "Content"),
                    context.FileOutputList));

            return new ModeledItem(document);
        }

        private class ZipContext
        {
            public string DestinationZipFile;
            public string SourceLobby;
            public IEnumerable<string> Items;

            public bool Flat;
            public bool Add;
            public bool Override;

            public bool ZipAlreadyExists;
            public List FileOutputList;
        }
    }
}
