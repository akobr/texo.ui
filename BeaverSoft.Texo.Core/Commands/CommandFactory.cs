using System;
using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Commands
{
    public class CommandFactory : ITexoFactory<object, string>
    {
        private readonly Dictionary<string, Func<object>> factories;

        public CommandFactory()
        {
            factories = new Dictionary<string, Func<object>>();
        }

        public object Create(string commandKey)
        {
            if (!factories.TryGetValue(commandKey, out Func<object> factory))
            {
                return null;
            }

            return factory.Invoke();
        }

        public void Release(object item)
        {
            (item as IDisposable)?.Dispose();
        }

        public void Register(string commandKey, Func<object> factory)
        {
            factories[commandKey] = factory;
        }

        public void Unregister(string commandKey)
        {
            factories.Remove(commandKey);
        }
    }
}