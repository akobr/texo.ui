using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Result;

namespace BeaverSoft.Texo.Core
{
    public class TexoCommand : ICommand
    {
        private readonly EnvironmentCommand environment;

        public TexoCommand(IEnvironmentService environment)
        {
            this.environment = new EnvironmentCommand(environment);
        }

        public ICommandResult Execute(CommandContext context)
        {
            switch (context.FirstQuery)
            {
                case EnvironmentNames.QUERY_ENVIRONMENT:
                    return environment.Execute(CommandContext.ShiftQuery(context));

                // case SettingNames.QUERY_SETTING:
                default:
                    return new TextResult("> Not implemented yet.");

            }
        }
    }
}
