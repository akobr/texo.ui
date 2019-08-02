namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class TexoConfiguration
    {
        public class Builder
        {
            internal Builder(TexoConfiguration immutable)
            {
                Runtime = immutable.runtime.ToBuilder();
                Environment = immutable.environment.ToBuilder();
                Ui = immutable.ui.ToBuilder();
            }

            public TexoRuntime.Builder Runtime { get; set; }

            public TexoEnvironment.Builder Environment { get; set; }

            public TexoUi.Builder Ui { get; set; }

            public TexoConfiguration ToImmutable()
            {
                return new TexoConfiguration(this);
            }
        }
    }
}
