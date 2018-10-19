namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public partial class TextumEnvironment
    {
        public sealed class Builder
        {
            internal Builder(TextumEnvironment immutable)
            {
                WorkingPath = immutable.workingPath;
            }

            public string WorkingPath { get; set; }

            public TextumEnvironment ToImmutable()
            {
                return new TextumEnvironment(this);
            }
        }
    }
}
