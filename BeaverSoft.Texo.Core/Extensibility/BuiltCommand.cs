using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Result;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Extensibility
{
    public class BuiltCommand : IAsyncCommand, IDisposable
    {
        private readonly Dictionary<string, Func<CommandContext, Task<ICommandResult>>> map;
        private readonly List<IDisposable> disposableContainer;

        public BuiltCommand()
        {
            map = new Dictionary<string, Func<CommandContext, Task<ICommandResult>>>(StringComparer.OrdinalIgnoreCase);
            disposableContainer = new List<IDisposable>();
        }

        public Task<ICommandResult> ExecuteAsync(CommandContext context)
        {
            if (!map.TryGetValue(context.FirstQuery, out var asyncExecutor))
            {
                return Task.FromResult<ICommandResult>(new ErrorTextResult($"No query method for {context.FirstQuery}."));
            }

            return asyncExecutor(context);
        }

        public void RegisterDisposable(IDisposable disposable)
        {
            disposableContainer.Add(disposable);
        }

        public void RegisterQuery(string queryKey, ICommand queryCommand)
        {
            map[queryKey] = (context) => { return Task.Run(() => queryCommand.Execute(context)); };
        }

        public void RegisterQuery(string queryKey, IAsyncCommand queryAsyncCommand)
        {
            map[queryKey] = (context) => { return queryAsyncCommand.ExecuteAsync(context); };
        }

        public void RegisterQuery(string queryKey, Func<CommandContext, Task<ICommandResult>> function)
        {
            map[queryKey] = function;
        }

        public void Dispose()
        {
            foreach (IDisposable disposable in disposableContainer)
            {
                disposable.Dispose();
            }

            disposableContainer.Clear();
        }
    }
}
