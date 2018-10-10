using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public partial class TextumRuntime
    {
        public sealed class Builder
        {
            internal Builder(TextumRuntime configuration)
            {
                Commands = configuration.commands;
                DefaultCommand = configuration.defaultCommand;
            }

            public ImmutableList<IQuery> Commands { get; set; }

            public string DefaultCommand { get; set; }

            public TextumRuntime ToImmutable()
            {
                return new TextumRuntime(this);
            }
        }
    }
}
