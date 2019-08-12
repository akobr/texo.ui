using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Inputting.History;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.Text;

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

                    if (context.HasOption("version"))
                    {
                        return new TextResult(TexoEngine.Version.ToString());
                    }

                    return BuildCommandHelp();

            }
        }

        // TODO: [P3] Change how the help works and automatically generate nice help per command
        public TextResult BuildCommandHelp()
        {
            AnsiStringBuilder builder = new AnsiStringBuilder();

            builder.Append("usage: ");
            builder.AppendForegroundFormat(System.ConsoleColor.Gray);
            builder.Append("texo [--version] <command> [<args>]");
            builder.AppendFormattingReset();
            builder.AppendLine();

            builder.AppendLine();
            builder.AppendForegroundFormat(System.ConsoleColor.Yellow);
            builder.Append("commands");
            builder.AppendFormattingReset();
            builder.AppendLine();

            builder.Append($"{EnvironmentNames.QUERY_ENVIRONMENT,-15}");
            builder.AppendForegroundFormat(System.ConsoleColor.Gray);
            builder.Append("Management of environment variables.");
            builder.AppendFormattingReset();
            builder.AppendLine();

            builder.Append($"{HistoryNames.QUERY_HISTORY,-15}");
            builder.AppendForegroundFormat(System.ConsoleColor.Gray);
            builder.Append("History of commands.");
            builder.AppendFormattingReset();
            builder.AppendLine();

            builder.AppendLine();
            builder.AppendForegroundFormat(System.ConsoleColor.Yellow);
            builder.Append("options");
            builder.AppendFormattingReset();
            builder.AppendLine();

            builder.Append($"{"-v--version",-15}");
            builder.AppendForegroundFormat(System.ConsoleColor.Gray);
            builder.Append("Prints out the version of the Texo UI in use.");
            builder.AppendFormattingReset();
            builder.AppendLine();

            return builder.ToString();
        }
}
}
