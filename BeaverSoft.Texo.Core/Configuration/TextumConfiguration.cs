namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class TextumConfiguration
    {
        private TextumRuntime runtime;
        private TextumEnvironment environment;
        private TextumUi ui;

        private TextumConfiguration()
        {
            runtime = TextumRuntime.Empty;
            environment = TextumEnvironment.Empty;
            ui = TextumUi.Empty;
        }

        public TextumConfiguration(TextumConfiguration toClone)
        {
            runtime = toClone.runtime;
            environment = toClone.environment;
            ui = toClone.ui;
        }

        public TextumConfiguration(Builder builder)
        {
            runtime = builder.Runtime.ToImmutable();
            environment = builder.Environment.ToImmutable();
            ui = builder.Ui.ToImmutable();
        }

        public TextumRuntime Runtime => runtime;

        public TextumEnvironment Environment => environment;

        public TextumUi Ui => ui;

        public TextumConfiguration SetRuntime(TextumRuntime value)
        {
            return new TextumConfiguration(this)
            {
                runtime = value
            };
        }

        public TextumConfiguration SetEnvironment(TextumEnvironment value)
        {
            return new TextumConfiguration(this)
            {
                environment = value
            };
        }

        public TextumConfiguration SetEnvironment(TextumUi value)
        {
            return new TextumConfiguration(this)
            {
                ui = value
            };
        }

        public Builder ToBuilder()
        {
            return new Builder(this);
        }
    }
}
