using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Commands
{
    public partial class OptionContext
    {
        public class Builder
        {
            internal Builder(OptionContext context)
            {
                Key = context.Key;
                Parameters = context.parameters.ToBuilder();
            }

            public string Key { get; set; }

            public ImmutableDictionary<string, IParameterContext>.Builder Parameters { get; }

            public OptionContext ToImmutable()
            {
                return new OptionContext(this);
            }
        }
    }
}
