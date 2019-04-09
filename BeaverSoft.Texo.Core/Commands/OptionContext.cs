using System.Collections.Immutable;
using System.Linq;

namespace BeaverSoft.Texo.Core.Commands
{
    public partial class OptionContext
    {
        private readonly ImmutableDictionary<string, ParameterContext> parameters;

        private OptionContext()
        {
            parameters = ImmutableDictionary<string, ParameterContext>.Empty;
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

        public ImmutableDictionary<string, ParameterContext> Parameters => parameters;

        public static OptionContext Empty { get; } = new OptionContext();

        public Builder ToBuilder()
        {
            return new Builder(this);
        }

        public string GetParameterValue(string parameterKey)
        {
            if (parameters.Count < 1
                || parameters.TryGetValue(parameterKey, out ParameterContext parameter))
            {
                return string.Empty;
            }

            return parameter.GetValue();
        }

        public string GetParameterValue()
        {
            ParameterContext parameter = parameters.Values.FirstOrDefault();
            return parameter?.GetValue() ?? string.Empty;
        }

        public static OptionContext BuildWithoutParameters(string key)
        {
            Builder builder = Empty.ToBuilder();
            builder.Key = key;
            return builder.ToImmutable();
        }
    }
}
