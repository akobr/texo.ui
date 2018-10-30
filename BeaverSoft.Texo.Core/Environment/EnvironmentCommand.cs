using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Environment
{
    public class EnvironmentCommand : ICommand
    {
        private readonly IEnvironmentService environment;

        public EnvironmentCommand(IEnvironmentService environment)
        {
            this.environment = environment;
        }

        public ICommandResult Execute(CommandContext context)
        {
            switch (context.FirstQuery)
            {
                case EnvironmentNames.QUERY_GET:
                    return GetVariable(context);

                case EnvironmentNames.QUERY_SET:
                    return SetVariable(context);

                case EnvironmentNames.QUERY_REMOVE:
                    return RemoveVariable(context);

                // case EnvironmentNames.QUERY_LIST:
                default:
                    return GetList();
            }
        }

        private ICommandResult GetList()
        {
            var result = ImmutableList<Item>.Empty.ToBuilder();
            foreach (var parameter in environment.GetVariables())
            {
                result.Add($"*{parameter.Key}* = {parameter.Value}");
            }

            return new ItemsResult(result.ToImmutable());
        }

        private ICommandResult GetVariable(CommandContext context)
        {
            string name = context.GetParameterValue(ParameterKeys.NAME);
            environment.GetVariable(name);
            return new TextResult(name);
        }

        private ICommandResult SetVariable(CommandContext context)
        {
            string name = context.GetParameterValue(ParameterKeys.NAME);
            string value = context.GetParameterValue(ParameterKeys.VALUE);
            environment.SetVariable(name, value);
            return new TextResult($"*{name}* = {value}");
        }

        private ICommandResult RemoveVariable(CommandContext context)
        {
            string name = context.GetParameterValue(ParameterKeys.NAME);
            environment.SetVariable(name, string.Empty);
            return new TextResult($"Variable *{name}* has been removed.");
        }
    }
}
