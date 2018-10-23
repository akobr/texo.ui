using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Commands.FileManager.Stash
{
    public class FileManagerStashCommand : ICommand
    {
        public ICommandResult Execute(ICommandContext context)
        {
            switch (context.Key)
            {
                case StashQueries.LIST:
                    break;

                case StashQueries.APPLY:
                    break;

                case StashQueries.PEAK:
                    break;

                case StashQueries.POP:
                    break;

                case StashQueries.DROP:
                    break;

                case StashQueries.CLEAR:
                    break;
            }
        }
    }
}
