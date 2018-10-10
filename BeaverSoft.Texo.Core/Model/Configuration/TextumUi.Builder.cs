namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public partial class TextumUi
    {
        public sealed class Builder
        {
            internal Builder(TextumUi configuration)
            {
                Prompt = configuration.prompt;
            }

            public string Prompt { get; set; }

            public bool ShowWorkingPathAsPrompt { get; set; }

            public TextumUi ToImmutable()
            {
                return new TextumUi(this);
            }
        }
    }
}
