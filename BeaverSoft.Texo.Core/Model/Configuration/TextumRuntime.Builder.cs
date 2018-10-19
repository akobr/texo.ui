using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public partial class TextumRuntime
    {
        public sealed class Builder
        {
            internal Builder(TextumRuntime immutable)
            {
                Commands = immutable.commands.ToBuilder();
                DefaultCommand = immutable.defaultCommand;
            }

            public ImmutableList<Query>.Builder Commands { get; }

            public string DefaultCommand { get; set; }

            public TextumRuntime ToImmutable()
            {
                return new TextumRuntime(this);
            }
        }
    }
}
