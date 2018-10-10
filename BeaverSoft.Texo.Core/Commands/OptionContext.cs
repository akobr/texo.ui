using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Commands
{
    public partial class OptionContext : IOptionContext
    {
        private readonly ImmutableDictionary<string, IParameterContext> parameters;

        private OptionContext()
        {
            parameters = ImmutableDictionary<string, IParameterContext>.Empty;
        }

        private OptionContext(OptionContext toClone)
        {
            Key = toClone.Key;
            parameters = toClone.parameters;
        }

        private OptionContext(Builder builder)
        {
            Key = builder.Key;
            parameters = builder.Parameters.ToImmutable();
        }

        public string Key { get; }

        public IImmutableDictionary<string, IParameterContext> Parameters => parameters;

        public static OptionContext Empty { get; } = new OptionContext();

        public Builder ToBuilder()
        {
            return new Builder(this);
        }

        public static OptionContext BuildWithoutParameters(string key)
        {
            Builder builder = Empty.ToBuilder();
            builder.Key = key;
            return builder.ToImmutable();
        }
    }
}
