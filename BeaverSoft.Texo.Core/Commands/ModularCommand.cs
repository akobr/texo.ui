using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Result;

namespace BeaverSoft.Texo.Core.Commands
{
    public abstract class ModularCommand : ICommand
    {
        private static readonly Type genericResultType = typeof(ICommandResult);
        private readonly Dictionary<string, Func<CommandContext, ICommandResult>> queries;

        protected ModularCommand()
        {
            queries = new Dictionary<string, Func<CommandContext, ICommandResult>>();
        }
        
        public ICommandResult Execute(CommandContext context)
        {
            if (!queries.TryGetValue(context.FirstQuery, out var subCommandProviderFunction))
            {
                return new ErrorTextResult($"No command for {context.FirstQuery}.");
            }

            return subCommandProviderFunction(CommandContext.ShiftQuery(context));
        }

        protected void RegisterQuery<TResult>(string key, Func<CommandContext, TResult> method)
        {
            if (genericResultType.IsAssignableFrom(typeof(TResult)))
            {
                queries[key] = (context) => (ICommandResult)method(context);
            }
            else
            {
                queries[key] = (context) => new Result<TResult>(method(context));
            }
        }

        protected void RegisterQuery<TResult>(string key, Func<CommandContext, Task<TResult>> asyncMethod)
        {
            queries[key] = (context) => new TaskResult<TResult>(asyncMethod(context));
        }

        protected void RegisterQuery(string key, Func<ICommand> subCommandProvider)
        {
            if (subCommandProvider == null)
            {
                throw new ArgumentNullException(nameof(subCommandProvider));
            }

            queries[key] = (context) => subCommandProvider()?.Execute(context);
        }

        protected void RegisterQuery(string key, ICommand subCommand)
        {
            if (subCommand == null)
            {
                throw new ArgumentNullException(nameof(subCommand));
            }

            queries[key] = (context) => subCommand.Execute(context);
        }      

        protected void UnregisterQuery(string key)
        {
            queries.Remove(key);
        }
    }
}
