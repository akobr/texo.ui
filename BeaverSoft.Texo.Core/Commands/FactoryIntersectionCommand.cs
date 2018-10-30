using System;
using System.Collections.Generic;
using BeaverSoft.Texo.Core.Result;

namespace BeaverSoft.Texo.Core.Commands
{
    public class FactoryIntersectionCommand : ICommand
    {
        private readonly Dictionary<string, Func<ICommand>> subCommands;

        public FactoryIntersectionCommand()
        {
            subCommands = new Dictionary<string, Func<ICommand>>();
        }

        public virtual ICommandResult Execute(CommandContext context)
        {
            if (!subCommands.TryGetValue(context.FirstQuery, out Func<ICommand> factory))
            {
                return new ErrorTextResult($"No command factory for {context.FirstQuery}.");
            }

            ICommand subCommand = factory();

            if(subCommand == null)
            {
                return new ErrorTextResult($"Null command for {context.FirstQuery}.");
            }

            return factory()?.Execute(CommandContext.ShiftQuery(context));
        }

        protected void RegisterCommand(string key, Func<ICommand> factory)
        {
            subCommands[key] = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        protected void UnregisterCommand(string key)
        {
            subCommands.Remove(key);
        }
    }
}
