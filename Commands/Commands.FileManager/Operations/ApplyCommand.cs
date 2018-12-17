using System;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Core.Commands;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Commands.FileManager.Operations
{
    public class ApplyCommand : IntersectionCommand
    {
        private readonly IStageService stage;
        private readonly ILogService logger;

        public ApplyCommand(IStageService stage, ILogService logger)
        {
            this.stage = stage ?? throw new ArgumentNullException(nameof(stage));
            this.logger = logger;

            RegisterCommand(ApplyQueries.COPY, new CopyCommand(stage, logger));
            RegisterCommand(ApplyQueries.MOVE, new MoveCommand(stage, logger));
            //RegisterCommand(ApplyQueries.RENAME, new RenameCommand());
            RegisterCommand(ApplyQueries.DELETE, new DeleteCommand(stage, logger));
            RegisterCommand(ApplyQueries.SEARCH, new ContentSearchCommand(stage, logger));
            //RegisterCommand(ApplyQueries.REPLACE, new ContentReplaceCommand());
            RegisterCommand(ApplyQueries.ARCHIVE, new ArchiveCommand(stage, logger));
        }
    }
}
