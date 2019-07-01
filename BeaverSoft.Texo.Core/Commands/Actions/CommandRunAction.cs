using System.Collections.Generic;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Runtime;

namespace BeaverSoft.Texo.Core.Commands.Actions
{
    public class CommandRunAction : IAction
    {
        private readonly IExecutor executor;

        public CommandRunAction(IExecutor executor)
        {
            this.executor = executor;
        }

        public Task ExecuteAsync(IDictionary<string, string> arguments)
        {
            if (!arguments.TryGetValue(ActionParameters.INPUT, out string input)
                || string.IsNullOrWhiteSpace(input))
            {
                return Task.CompletedTask;
            }

            return executor.ProcessAsync(input);
        }
    }
}
