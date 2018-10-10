using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Commands
{
    public interface IAsyncCommand : ICommand
    {
        Task<ICommandResult> ExecuteAsync(ICommandContext context);
    }
}