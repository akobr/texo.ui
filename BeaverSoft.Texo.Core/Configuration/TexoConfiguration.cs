using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class TexoConfiguration
    {
        private TexoRuntime runtime;
        private TexoEnvironment environment;
        private TexoUi ui;

        private TexoConfiguration()
        {
            runtime = TexoRuntime.Empty;
            environment = TexoEnvironment.Empty;
            ui = TexoUi.Empty;
        }

        public TexoConfiguration(TexoConfiguration toClone)
        {
            runtime = toClone.runtime;
            environment = toClone.environment;
            ui = toClone.ui;
        }

        public TexoConfiguration(Builder builder)
        {
            runtime = builder.Runtime.ToImmutable();
            environment = builder.Environment.ToImmutable();
            ui = builder.Ui.ToImmutable();
        }

        public TexoRuntime Runtime => runtime;

        public TexoEnvironment Environment => environment;

        public TexoUi Ui => ui;

        public TexoConfiguration SetRuntime(TexoRuntime value)
        {
            return new TexoConfiguration(this)
            {
                runtime = value
            };
        }

        public TexoConfiguration SetEnvironment(TexoEnvironment value)
        {
            return new TexoConfiguration(this)
            {
                environment = value
            };
        }

        public TexoConfiguration SetEnvironment(TexoUi value)
        {
            return new TexoConfiguration(this)
            {
                ui = value
            };
        }

        public TexoConfiguration AddCommands(IEnumerable<Query> commands)
        {
            return new TexoConfiguration(this)
            {
                runtime = runtime.AddCommands(commands)
            };
        }

        public Builder ToBuilder()
        {
            return new Builder(this);
        }
    }
}
