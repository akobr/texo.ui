using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Inputting.History;
using BeaverSoft.Texo.Core.Result;

namespace BeaverSoft.Texo.Core
{
    public class TexoCommand : ICommand
    {
        private readonly EnvironmentCommand environment;
        private readonly HistoryCommand history;

        public TexoCommand(
            IEnvironmentService environment,
            IInputHistoryService history)
        {
            this.environment = new EnvironmentCommand(environment);
            this.history = new HistoryCommand(history);
        }

        public ICommandResult Execute(CommandContext context)
        {
            switch (context.FirstQuery)
            {
                case EnvironmentNames.QUERY_ENVIRONMENT:
                    return environment.Execute(CommandContext.ShiftQuery(context));

                case HistoryNames.QUERY_HISTORY:
                    return history.Execute(CommandContext.ShiftQuery(context));

                default:
                    return new ErrorTextResult("Not implemented yet.");

            }
        }
    }
}
