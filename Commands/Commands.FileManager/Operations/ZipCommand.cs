using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Result;

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
                SourceLobby = stage.GetLobby(),
                Flat = context.HasOption(ApplyOptions.FLATTEN) || !stage.HasLobby(),
                Override = context.HasOption(ApplyOptions.OVERRIDE),
                Preview = context.HasOption(ApplyOptions.PREVIEW)
            });
        }

        private ICommandResult Zip(ZipContext zipContext)
        {
            if (File.Exists(zipContext.DestinationZipFile)
                && !zipContext.Add 
                && !zipContext.Override)
            {
                return new ErrorTextResult("The zip file already exists.");
            }

            using (FileStream zipStream = new FileStream(zipContext.DestinationZipFile, FileAccess.Write))
            using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
            {

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
        }
    }
}
