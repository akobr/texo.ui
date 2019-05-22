using BeaverSoft.Texo.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Actions.Implementations
{
    public class CommandRunAction : IAction
    {
        private readonly IExecutor executor;

        public CommandRunAction(IExecutor executor)
        {
            this.executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public Task ExecuteAsync(IDictionary<string, string> arguments)
        {
            if (!arguments.TryGetValue(ActionParameters.STATEMENT, out string statement))
            {
                return Task.CompletedTask;
            }

            return executor.ProcessAsync(statement);
        }
    }
}
