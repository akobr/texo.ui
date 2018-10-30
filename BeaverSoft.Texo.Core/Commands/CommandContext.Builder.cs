using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Commands
{
    public partial class CommandContext
    {
        public class Builder
        {
            internal Builder(CommandContext context)
            {
                Key = context.Key;
                IsValid = context.IsValid;
                QueryPath = context.queryPath.ToBuilder();
                Options = context.options.ToBuilder();
                Parameters = context.parameters.ToBuilder();
            }

            public string Key { get; set; }

            public bool IsValid { get; set; }

            public ImmutableList<string>.Builder QueryPath { get; }

            public ImmutableDictionary<string, OptionContext>.Builder Options { get; }

            public ImmutableDictionary<string, ParameterContext>.Builder Parameters { get; }

            public CommandContext ToImmutable()
            {
                return new CommandContext(this);
            }
        }
    }
}
