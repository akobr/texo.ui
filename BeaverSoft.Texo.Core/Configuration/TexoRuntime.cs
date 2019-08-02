using System.Collections.Generic;
using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Configuration
{
    public sealed partial class TexoRuntime
    {
        private ImmutableList<Query> commands;
        private string defaultCommand;

        private TexoRuntime()
        {
            commands = ImmutableList<Query>.Empty;
        }

        private TexoRuntime(TexoRuntime toClone)
        {
            commands = toClone.commands;
            defaultCommand = toClone.defaultCommand;
        }

        private TexoRuntime(Builder builder)
        {
            commands = builder.Commands.ToImmutable();
            defaultCommand = builder.DefaultCommand;
        }

        public ImmutableList<Query> Commands => commands;

        public string DefaultCommand => defaultCommand;

        public TexoRuntime SetCommands(ImmutableList<Query> value)
        {
            return new TexoRuntime(this)
            {
                commands = value
            };
        }

        public TexoRuntime SetDefaultCommand(string value)
        {
            return new TexoRuntime(this)
            {
                defaultCommand = value
            };
        }

        public TexoRuntime AddCommands(IEnumerable<Query> newCommands)
        {
            if (commands == null)
            {
                return this;
            }

            var builder = commands.ToBuilder();
            builder.AddRange(newCommands);
            return new TexoRuntime(this)
            {
                commands = builder.ToImmutable()
            };
        }

        public Builder ToBuilder()
        {
            return new Builder(this);
        }
    }
}