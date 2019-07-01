using System;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Commands.FileManager.Stash;
using BeaverSoft.Texo.Core.Commands;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Commands.FileManager.Operations
{
    public class ApplyCommand : IntersectionCommand
    {
        private readonly IStageService stage;
        private readonly IStashService stashes;
        private readonly ILogService logger;

        public ApplyCommand(IStageService stage, IStashService stashes, ILogService logger)
        {
            this.stage = stage ?? throw new ArgumentNullException(nameof(stage));
            this.stashes = stashes ?? throw new ArgumentNullException(nameof(stashes));
            this.logger = logger;

            RegisterCommand(ApplyQueries.COPY, new CopyCommand(stage, stashes, logger));
            RegisterCommand(ApplyQueries.MOVE, new MoveCommand(stage, stashes, logger));
            //RegisterCommand(ApplyQueries.RENAME, new RenameCommand());
            RegisterCommand(ApplyQueries.DELETE, new DeleteCommand(stage, stashes, logger));
            RegisterCommand(ApplyQueries.SEARCH, new ContentSearchCommand(stage, stashes, logger));
            //RegisterCommand(ApplyQueries.REPLACE, new ContentReplaceCommand());
            RegisterCommand(ApplyQueries.ARCHIVE, new ArchiveCommand(stage, stashes, logger));
        }
    }
}
