using System;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Commands.FileManager.Operations
{
    public class ApplyCommand : IntersectionCommand
    {
        private readonly IStageService stage;

        public ApplyCommand(IStageService stage)
        {
            this.stage = this.stage ?? throw new ArgumentNullException(nameof(stage));
        }

        public ApplyCommand()
        {
            RegisterCommand(ApplyQueries.COPY, new CopyCommand(stage));
            RegisterCommand(ApplyQueries.MOVE, new MoveCommand());
            RegisterCommand(ApplyQueries.RENAME, new RenameCommand());
            RegisterCommand(ApplyQueries.DELETE, new DeleteCommand());
            RegisterCommand(ApplyQueries.SEARCH, new ContentSearchCommand());
            RegisterCommand(ApplyQueries.REPLACE, new ContentReplaceCommand());
            RegisterCommand(ApplyQueries.ARCHIVE, new ZipCommand());
        }
    }
}
