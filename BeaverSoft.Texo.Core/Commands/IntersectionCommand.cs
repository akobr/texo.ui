using System;
using System.Collections.Generic;
using BeaverSoft.Texo.Core.Result;

namespace BeaverSoft.Texo.Core.Commands
{
    public abstract class IntersectionCommand : ICommand
    {
        private readonly Dictionary<string, ICommand> subCommands;

        protected IntersectionCommand()
        {
            subCommands = new Dictionary<string, ICommand>();
        }

        public virtual ICommandResult Execute(CommandContext context)
        {
            if (!subCommands.TryGetValue(context.FirstQuery, out ICommand subCommand))
            {
                return new ErrorTextResult($"No command for {context.FirstQuery}.");
            }

            return subCommand.Execute(CommandContext.ShiftQuery(context));
        }

        protected void RegisterCommand(string key, ICommand subCommand)
        {
            subCommands[key] = subCommand ?? throw new ArgumentNullException(nameof(subCommand));
        }

        protected void UnregisterCommand(string key)
        {
            subCommands.Remove(key);
        }
    }
}
