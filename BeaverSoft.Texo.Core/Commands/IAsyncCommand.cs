using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Commands
{
    public interface IAsyncCommand
    {
        Task<ICommandResult> ExecuteAsync(CommandContext context);
    }
}