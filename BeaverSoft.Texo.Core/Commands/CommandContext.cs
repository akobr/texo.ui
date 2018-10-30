using System.Collections.Immutable;
using System.Linq;

namespace BeaverSoft.Texo.Core.Commands
{
    public partial class CommandContext
    {
        private readonly ImmutableList<string> queryPath;
        private readonly ImmutableDictionary<string, OptionContext> options;
        private readonly ImmutableDictionary<string, ParameterContext> parameters;

        private CommandContext()
        {
            queryPath = ImmutableList<string>.Empty;
            options = ImmutableDictionary<string, OptionContext>.Empty;
            parameters = ImmutableDictionary<string, ParameterContext>.Empty;
        }

        private CommandContext(CommandContext toClone)
        {
            Key = toClone.Key;
            IsValid = toClone.IsValid;
            queryPath = toClone.queryPath;
            options = toClone.options;
            parameters = toClone.parameters;
        }

        private CommandContext(Builder builder)
        {
            Key = builder.Key;
            IsValid = builder.IsValid;
            queryPath = builder.QueryPath.ToImmutable();
            options = builder.Options.ToImmutable();
            parameters = builder.Parameters.ToImmutable();
        }

        public string Key { get; }

        public bool IsValid { get; }

        public ImmutableList<string> QueryPath => queryPath;

        public ImmutableDictionary<string, OptionContext> Options => options;

        public ImmutableDictionary<string, ParameterContext> Parameters => parameters;

        public static CommandContext Empty { get; } = new CommandContext();

        public string FirstQuery => QueryPath.FirstOrDefault() ?? string.Empty;

        public string GetParameterValue(string parameterName)
        {
            if (!Parameters.TryGetValue(parameterName, out ParameterContext parameter))
            {
                return string.Empty;
            }

            return parameter.GetValue();
        }

        public IImmutableList<string> GetParameterValues(string parameterName)
        {
            if (!Parameters.TryGetValue(parameterName, out ParameterContext parameter))
            {
                return ImmutableList<string>.Empty;
            }

            return parameter.GetValues();
        }

        public Builder ToBuilder()
        {
            return new Builder(this);
        }
    }
}
