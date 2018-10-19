using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public sealed partial class TextumRuntime
    {
        private ImmutableList<Query> commands;
        private string defaultCommand;

        private TextumRuntime()
        {
            commands = ImmutableList<Query>.Empty;
        }

        private TextumRuntime(TextumRuntime toClone)
        {
            commands = toClone.commands;
            defaultCommand = toClone.defaultCommand;
        }

        private TextumRuntime(Builder builder)
        {
            commands = builder.Commands.ToImmutable();
            defaultCommand = builder.DefaultCommand;
        }

        public ImmutableList<Query> Commands => commands;

        public string DefaultCommand => defaultCommand;

        public TextumRuntime SetCommands(ImmutableList<Query> value)
        {
            return new TextumRuntime(this)
            {
                commands = value
            };
        }

        public TextumRuntime SetDefaultCommand(string value)
        {
            return new TextumRuntime(this)
            {
                defaultCommand = value
            };
        }

        public Builder ToBuilder()
        {
            return new Builder(this);
        }
    }
}