using System.Collections.Generic;
using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Commands
{
    public partial class ParameterContext
    {
        private const int FIRST_INDEX = 0;
        private readonly ImmutableList<string> values;

        private ParameterContext()
        {
            values = ImmutableList<string>.Empty;
        }

        public ParameterContext(string key, IEnumerable<string> values)
            : this()
        {
            Key = key;
            this.values = ImmutableList<string>.Empty.AddRange(values);
        }

        public ParameterContext(string key, string value)
            : this(key, new[] { value })
        {
            // no operation
        }

        private ParameterContext(Builder builder)
        {
            Key = builder.Key;
            values = builder.Values.ToImmutable();
        }

        public string Key { get; }

        public static ParameterContext Empty { get; } = new ParameterContext();

        public string GetValue()
        {
            return values[FIRST_INDEX] ?? string.Empty;
        }

        public IImmutableList<string> GetValues()
        {
            return values;
        }

        public Builder ToBuilder()
        {
            return new Builder(this);
        }

        public static ParameterContext Build(string parameterKey, string tokenValue)
        {
            Builder builder = Empty.ToBuilder();
            builder.Key = parameterKey;
            builder.Values.Add(tokenValue);
            return builder.ToImmutable();
        }
    }
}
