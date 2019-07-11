using System;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Commands.FileManager.Stash;
using BeaverSoft.Texo.Core.Commands;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Commands.FileManager.Operations
{
    public class ApplyCommand : ModularCommand
    {
        private readonly IStageService stage;
        private readonly IStashService stashes;
        private readonly ILogService logger;

        public ApplyCommand(IStageService stage, IStashService stashes, ILogService logger)
        {
            this.stage = stage ?? throw new ArgumentNullException(nameof(stage));
            this.stashes = stashes ?? throw new ArgumentNullException(nameof(stashes));
            this.logger = logger;

            RegisterQuery(ApplyQueries.COPY, new CopyCommand(stage, stashes, logger));
            RegisterQuery(ApplyQueries.MOVE, new MoveCommand(stage, stashes, logger));
            //RegisterQuery(ApplyQueries.RENAME, new RenameCommand());
            RegisterQuery(ApplyQueries.DELETE, new DeleteCommand(stage, stashes, logger));
            RegisterQuery(ApplyQueries.SEARCH, new ContentSearchCommand(stage, stashes, logger));
            //RegisterQuery(ApplyQueries.REPLACE, new ContentReplaceCommand());
            RegisterQuery(ApplyQueries.ARCHIVE, new ArchiveCommand(stage, stashes, logger));
        }
    }
}
