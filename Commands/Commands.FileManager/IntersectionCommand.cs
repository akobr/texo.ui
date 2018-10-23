using System;
using System.Collections.Generic;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Result;

namespace BeaverSoft.Texo.Commands.FileManager
{
    public abstract class IntersectionCommand : ICommand
    {
        private readonly Dictionary<string, ICommand> subCommands;

        public IntersectionCommand()
        {
            subCommands = new Dictionary<string, ICommand>();
        }

        public virtual ICommandResult Execute(ICommandContext context)
        {
            if (!subCommands.TryGetValue(context.Key, out ICommand subCommand))
            {
                return new ErrorTextResult($"No command for {context.Key}.");
            }

            return subCommand.Execute(context); // TODO: shift
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
