namespace BeaverSoft.Texo.Core.Commands
{
    public interface ICommand
    {
        ICommandResult Execute(CommandContext context);
    }
}
