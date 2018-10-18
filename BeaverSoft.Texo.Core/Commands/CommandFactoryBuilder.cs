using System;

namespace BeaverSoft.Texo.Core.Commands
{
    public class CommandFactoryBuilder
    {
        private readonly CommandFactory factory;

        public CommandFactoryBuilder()
        {
            factory = new CommandFactory();
        }

        public CommandFactoryBuilder WithCommand(string commandKey, ICommand command)
        {
            factory.Register(commandKey, () => command);
            return this;
        }

        public CommandFactoryBuilder WithCommand(string commandKey, Func<ICommand> methodFactory)
        {
            factory.Register(commandKey, methodFactory);
            return this;
        }

        public CommandFactoryBuilder WithCommand(string commandKey, Lazy<ICommand> lazyCommand)
        {
            factory.Register(commandKey, () => lazyCommand.Value);
            return this;
        }

        public CommandFactory Build()
        {
            return factory;
        }
    }
}
