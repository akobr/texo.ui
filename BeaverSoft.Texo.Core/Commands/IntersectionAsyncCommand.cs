using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Result;

namespace BeaverSoft.Texo.Core.Commands
{
    public class IntersectionAsyncCommand : IAsyncCommand
    {
        private readonly Dictionary<string, IAsyncCommand> subCommands;

        protected IntersectionAsyncCommand()
        {
            subCommands = new Dictionary<string, IAsyncCommand>();
        }

        public virtual Task<ICommandResult> ExecuteAsync(CommandContext context)
        {
            if (!subCommands.TryGetValue(context.FirstQuery, out IAsyncCommand subCommand))
            {
                return Task.FromResult< ICommandResult>(
                    new ErrorTextResult($"No command for {context.FirstQuery}."));
            }

            return subCommand.ExecuteAsync(CommandContext.ShiftQuery(context));
        }

        protected void RegisterCommand(string key, IAsyncCommand subCommand)
        {
            subCommands[key] = subCommand ?? throw new ArgumentNullException(nameof(subCommand));
        }

        protected void UnregisterCommand(string key)
        {
            subCommands.Remove(key);
        }
    }
}
