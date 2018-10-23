using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class TextumEnvironment
    {
        public sealed class Builder
        {
            internal Builder(TextumEnvironment immutable)
            {
                Variables = immutable.variables.ToBuilder();
            }

            public ImmutableDictionary<string, string>.Builder Variables { get; set; }

            public TextumEnvironment ToImmutable()
            {
                return new TextumEnvironment(this);
            }
        }
    }
}
