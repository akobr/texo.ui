using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class TexoEnvironment
    {
        public sealed class Builder
        {
            internal Builder(TexoEnvironment immutable)
            {
                Variables = immutable.variables.ToBuilder();
            }

            public ImmutableDictionary<string, string>.Builder Variables { get; set; }

            public TexoEnvironment ToImmutable()
            {
                return new TexoEnvironment(this);
            }
        }
    }
}
