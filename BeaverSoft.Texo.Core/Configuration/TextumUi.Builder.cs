namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class TextumUi
    {
        public sealed class Builder
        {
            internal Builder(TextumUi immutable)
            {
                Prompt = immutable.prompt;
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
