namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public partial class TextumConfiguration
    {
        public class Builder
        {
            internal Builder(TextumConfiguration immutable)
            {
                Runtime = immutable.runtime.ToBuilder();
                Environment = immutable.environment.ToBuilder();
                Ui = immutable.ui.ToBuilder();
            }

            public TextumRuntime.Builder Runtime { get; set; }

            public TextumEnvironment.Builder Environment { get; set; }

            public TextumUi.Builder Ui { get; set; }

            public TextumConfiguration ToImmutable()
            {
                return new TextumConfiguration(this);
            }
        }
    }
}
