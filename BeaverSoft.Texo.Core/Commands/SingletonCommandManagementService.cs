using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Commands
{
    public class SingletonCommandManagementService : ICommandManagementService
    {
        private readonly ITexoFactory<ICommand, string> factory;
        private readonly Dictionary<string, ICommand> commands;

        public SingletonCommandManagementService(ITexoFactory<ICommand, string> factory)
        {
            this.factory = factory;
            commands = new Dictionary<string, ICommand>();
        }

        public bool HasCommand(string commandKey)
        {
            if (commands.ContainsKey(commandKey))
            {
                return true;
            }

            return CreateCommand(commandKey) != null;
        }

        public ICommand BuildCommand(string commandKey)
        {
            if (commands.TryGetValue(commandKey, out ICommand command))
            {
                return command;
            }

            return CreateCommand(commandKey);
        }

        public void Dispose()
        {
            foreach (ICommand command in commands.Values)
            {
                factory.Release(command);
            }

            commands.Clear();
        }

        private ICommand CreateCommand(string commandKey)
        {
            ICommand command = factory.Create(commandKey);

            if (command == null)
            {
                return null;
            }

            commands[commandKey] = command;
            return command;
        }
    }
}
