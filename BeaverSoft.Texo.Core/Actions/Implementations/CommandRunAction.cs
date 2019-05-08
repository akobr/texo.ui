using BeaverSoft.Texo.Core.Runtime;
using System;
using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Actions.Implementations
{
    public class CommandRunAction : IAction
    {
        private readonly IExecutor executor;

        public CommandRunAction(IExecutor executor)
        {
            this.executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public async void Execute(IDictionary<string, string> arguments)
        {
            if (!arguments.TryGetValue(ActionParameters.STATEMENT, out string statement))
            {
                return;
            }

            await executor.ProcessAsync(statement);
        }
    }
}
