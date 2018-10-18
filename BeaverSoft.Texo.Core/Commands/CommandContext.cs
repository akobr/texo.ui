using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Commands
{
    public partial class CommandContext : ICommandContext
    {
        private readonly ImmutableList<string> queryPath;
        private readonly ImmutableDictionary<string, IOptionContext> options;
        private readonly ImmutableDictionary<string, IParameterContext> parameters;

        private CommandContext()
        {
            options = ImmutableDictionary<string, IOptionContext>.Empty;
            parameters = ImmutableDictionary<string, IParameterContext>.Empty;
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

        public IImmutableList<string> QueryPath { get; }

        public IImmutableDictionary<string, IOptionContext> Options => options;

        public IImmutableDictionary<string, IParameterContext> Parameters => parameters;

        public static CommandContext Empty { get; } = new CommandContext();

        public Builder ToBuilder()
        {
            return new Builder(this);
        }
    }
}
