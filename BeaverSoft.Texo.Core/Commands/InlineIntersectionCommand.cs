using System;
using System.Collections.Generic;
using BeaverSoft.Texo.Core.Result;

namespace BeaverSoft.Texo.Core.Commands
{
    public abstract class InlineIntersectionCommand : ICommand
    {
        private Dictionary<string, Func<CommandContext, ICommandResult>> map;

        protected InlineIntersectionCommand()
        {
            map = new Dictionary<string, Func<CommandContext, ICommandResult>>();
        }

        public virtual ICommandResult Execute(CommandContext context)
        {
            if (!map.TryGetValue(context.FirstQuery, out Func<CommandContext, ICommandResult> method))
            {
                return new ErrorTextResult($"No query method for {context.FirstQuery}.");
            }

            return method.Invoke(context);
        }

        protected void RegisterQueryMethod(string queryKey, Func<CommandContext, ICommandResult> method)
        {
            map[queryKey] = method;
        }

        protected  void UnregisterQueryMethod(string queryKey)
        {
            map.Remove(queryKey);
        }
    }
}