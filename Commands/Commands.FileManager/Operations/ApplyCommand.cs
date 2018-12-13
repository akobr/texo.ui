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
        }

        public ApplyCommand()
        {
            RegisterCommand(ApplyQueries.COPY, new CopyCommand(stage));
            RegisterCommand(ApplyQueries.MOVE, new MoveCommand(stage));
            //RegisterCommand(ApplyQueries.RENAME, new RenameCommand());
            RegisterCommand(ApplyQueries.DELETE, new DeleteCommand(stage));
            RegisterCommand(ApplyQueries.SEARCH, new ContentSearchCommand(stage, logger));
            //RegisterCommand(ApplyQueries.REPLACE, new ContentReplaceCommand());
            RegisterCommand(ApplyQueries.ARCHIVE, new ZipCommand(stage));
        }
    }
}
