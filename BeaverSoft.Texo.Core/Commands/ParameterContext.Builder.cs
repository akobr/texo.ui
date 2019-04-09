using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Commands
{
    public partial class ParameterContext
    {
        public class Builder
        {
            public Builder(ParameterContext context)
            {
                Key = context.Key;
                Values = context.values.ToBuilder();
            }

            public string Key { get; set; }

            public ImmutableList<string>.Builder Values { get; }

            public ParameterContext ToImmutable()
            {
                return new ParameterContext(this);
            }
        }
    }
}
