using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Commands.FileManager.Operations
{
    public class ApplyCommand : IntersectionCommand
    {
        public ApplyCommand()
        {
            RegisterCommand(ApplyQueries.COPY, new CopyCommand());
            RegisterCommand(ApplyQueries.MOVE, new MoveCommand());
            RegisterCommand(ApplyQueries.RENAME, new RenameCommand());
            RegisterCommand(ApplyQueries.DELETE, new DeleteCommand());
            RegisterCommand(ApplyQueries.SEARCH, new ContentSearchCommand());
            RegisterCommand(ApplyQueries.REPLACE, new ContentReplaceCommand());
            RegisterCommand(ApplyQueries.ARCHIVE, new ZipCommand());
        }
    }
}
