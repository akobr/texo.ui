using System;
using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Commands
{
    public class CommandFactory : ITexoFactory<ICommand, string>
    {
        private readonly Dictionary<string, Func<ICommand>> factories;

        public CommandFactory()
        {
            factories = new Dictionary<string, Func<ICommand>>();
        }

        public ICommand Create(string commandKey)
        {
            if (!factories.TryGetValue(commandKey, out Func<ICommand> factory))
            {
                return null;
            }

            return factory.Invoke();
        }

        public void Release(ICommand item)
        {
            (item as IDisposable)?.Dispose();
        }

        public void Register(string commandKey, Func<ICommand> factory)
        {
            factories[commandKey] = factory;
        }

        public void Unregister(string commandKey)
        {
            factories.Remove(commandKey);
        }
    }
}