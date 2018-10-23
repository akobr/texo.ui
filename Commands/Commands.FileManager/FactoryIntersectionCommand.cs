using System;
using System.Collections.Generic;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Result;

namespace BeaverSoft.Texo.Commands.FileManager
{
    public class FactoryIntersectionCommand : ICommand
    {
        private readonly Dictionary<string, Func<ICommand>> subCommands;

        public FactoryIntersectionCommand()
        {
            subCommands = new Dictionary<string, Func<ICommand>>();
        }

        public virtual ICommandResult Execute(ICommandContext context)
        {
            if (!subCommands.TryGetValue(context.Key, out Func<ICommand> factory))
            {
                return new ErrorTextResult($"No command factory for {context.Key}.");
            }

            ICommand subCommand = factory();

            if(subCommand == null)
            {
                return new ErrorTextResult($"Null command for {context.Key}.");
            }

            return factory()?.Execute(context); // TODO: shift
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
