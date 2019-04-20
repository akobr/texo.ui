using BeaverSoft.Texo.Core.Runtime;
using System;

namespace BeaverSoft.Texo.Core.Actions.Implementations
{
    public class CommandRunActionFactory : IActionFactory
    {
        private readonly IExecutor executor;

        public CommandRunActionFactory(IExecutor executor)
        {
            this.executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public IAction Build()
        {
            return new CommandRunAction(executor);
        }
    }
}
