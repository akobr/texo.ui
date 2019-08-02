using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class TexoRuntime
    {
        public sealed class Builder
        {
            internal Builder(TexoRuntime immutable)
            {
                Commands = immutable.commands.ToBuilder();
                DefaultCommand = immutable.defaultCommand;
            }

            public ImmutableList<Query>.Builder Commands { get; }

            public string DefaultCommand { get; set; }

            public TexoRuntime ToImmutable()
            {
                return new TexoRuntime(this);
            }
        }
    }
}
