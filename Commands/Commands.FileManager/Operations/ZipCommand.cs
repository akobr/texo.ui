using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Model.Text;
using BeaverSoft.Texo.Core.Path;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Commands.FileManager.Operations
{
    public class ZipCommand : ICommand
    {
        private readonly IStageService stage;

        public ZipCommand(IStageService stage)
        {
            this.stage = stage ?? throw new ArgumentNullException(nameof(stage));
        }

        public ICommandResult Execute(CommandContext context)
        {
            if (!stage.TryGetPaths(out var paths))
            {
                return new TextResult("The stage is empty.");
            }

            return Zip(new ZipContext
            {
                Items = paths,
                DestinationZipFile = context.GetTargetFile(),
                SourceLobby = stage.GetLobby(),
                Flat = context.HasOption(ApplyOptions.FLATTEN) || !stage.HasLobby(),
                Override = context.HasOption(ApplyOptions.OVERRIDE),
                Add = context.HasOption(ApplyOptions.ADD),
                Preview = context.HasOption(ApplyOptions.PREVIEW)
            });
        }

        private static ICommandResult Zip(ZipContext context)
        {
            context.ZipAlreadyExists = File.Exists(context.DestinationZipFile);

            if (context.ZipAlreadyExists
                && !context.Add
                && !context.Override)
            {
                return new ItemsResult(
                    Item.Markdown("The zip file already exists, use options `--override` or `--add`."));
            }

            context.FileOutputList = new List();

            using (FileStream zipStream = new FileStream(context.DestinationZipFile, FileMode.OpenOrCreate, FileAccess.Write))
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

            Document document = new Document(
                new Header(context.DestinationZipFile.GetFileNameOrDirectoryName()),
                new Paragraph(new Core.Model.Text.Link(
                    context.DestinationZipFile.GetFullPath(),
                    $"action://open-file?path={Uri.EscapeUriString(context.DestinationZipFile.GetFullPath())}")),
                new Section(
                    new Header(2, "Content"),
                    context.FileOutputList));

            return new ItemsResult<ModeledItem>(new ModeledItem(document));
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

            bool flatten = context.Flat || filePath.IsSubPathOf(context.SourceLobby);
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
                if (!context.Add)
                {
                    return;
                }

                fileEntry = archive.CreateEntry(destinationPath);
            }

            context.FileOutputList = context.FileOutputList.AddItem(
                new ListItem(new Core.Model.Text.Link(
                    context.DestinationZipFile.GetFullPath(),
                    $"action://open-file?path={Uri.EscapeUriString(context.DestinationZipFile.GetFullPath())}")));

            using (FileStream originalStream = new FileStream(filePath, FileMode.Open))
            using (Stream targetStream = fileEntry.Open())
            {
                originalStream.CopyTo(targetStream);
            }
        }

        private class ZipContext
        {
            public string DestinationZipFile;
            public string SourceLobby;
            public IEnumerable<string> Items;

            public bool Flat;
            public bool Add;
            public bool Override;
            public bool Preview;

            public bool ZipAlreadyExists;
            public List FileOutputList;
        }
    }
}
