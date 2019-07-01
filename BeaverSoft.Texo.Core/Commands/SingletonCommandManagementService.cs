using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Commands
{
    public class SingletonCommandManagementService : ICommandManagementService
    {
        private readonly ITexoFactory<object, string> factory;
        private readonly Dictionary<string, object> commands;

        public SingletonCommandManagementService(ITexoFactory<object, string> factory)
        {
            this.factory = factory;
            commands = new Dictionary<string, object>();
        }

        public bool HasCommand(string commandKey)
        {
            if (commands.ContainsKey(commandKey))
            {
                return true;
            }

            return CreateCommand(commandKey) != null;
        }

        public object BuildCommand(string commandKey)
        {
            if (commands.TryGetValue(commandKey, out object command))
            {
                return command;
            }

            return CreateCommand(commandKey);
        }

        public void Dispose()
        {
            foreach (object command in commands.Values)
            {
                factory.Release(command);
            }

            commands.Clear();
        }

        private object CreateCommand(string commandKey)
        {
            object command = factory.Create(commandKey);

            if (command == null)
            {
                return null;
            }

            commands[commandKey] = command;
            return command;
        }
    }
}
