using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Result;

namespace BeaverSoft.Texo.Core.Commands
{
    public abstract class InlineIntersectionAsyncCommand : IAsyncCommand
    {
        private Dictionary<string, Func<CommandContext, Task<ICommandResult>>> map;

        protected InlineIntersectionAsyncCommand()
        {
            map = new Dictionary<string, Func<CommandContext, Task<ICommandResult>>>();
        }

        public Task<ICommandResult> ExecuteAsync(CommandContext context)
        {
            if (!map.TryGetValue(context.FirstQuery, out Func<CommandContext, Task<ICommandResult>> method))
            {
                return Task.FromResult<ICommandResult>(
                    new ErrorTextResult($"No query method for {context.FirstQuery}."));
            }

            return method.Invoke(context);
        }

        protected void RegisterQueryMethod(string queryKey, Func<CommandContext, Task<ICommandResult>> method)
        {
            map[queryKey] = method;
        }

        protected void UnregisterQueryMethod(string queryKey)
        {
            map.Remove(queryKey);
        }
    }
}
