using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Result;

namespace BeaverSoft.Texo.Core.Commands
{
    public abstract class AsyncCommand<TResult> : ICommand
    {
        public ICommandResult Execute(CommandContext context)
        {
            return new TaskResult<TResult>(ExecuteAsync(context));
        }

        protected abstract Task<TResult> ExecuteAsync(CommandContext context);
    }
}
