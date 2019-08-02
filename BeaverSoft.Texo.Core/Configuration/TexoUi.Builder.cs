namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class TexoUi
    {
        public sealed class Builder
        {
            internal Builder(TexoUi immutable)
            {
                Prompt = immutable.prompt;
            }

            public string Prompt { get; set; }

            public bool ShowWorkingPathAsPrompt { get; set; }

            public TexoUi ToImmutable()
            {
                return new TexoUi(this);
            }
        }
    }
}
