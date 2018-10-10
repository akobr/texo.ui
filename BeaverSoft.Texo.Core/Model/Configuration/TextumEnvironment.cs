namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public partial class TextumEnvironment : ITextumEnvironment
    {
        private string workingPath;

        private TextumEnvironment()
        {
            workingPath = string.Empty;
        }

        private TextumEnvironment(TextumEnvironment configuration)
        {
            workingPath = configuration.workingPath;
        }

        private TextumEnvironment(Builder builder)
        {
            workingPath = builder.WorkingPath;
        }

        public string WorkingPath => workingPath;

        public TextumEnvironment SetWorkingPath(string value)
        {
            return new TextumEnvironment(this)
            {
                workingPath = value
            };
        }

        public Builder ToBuilder()
        {
            return new Builder(this);
        }
    }
}
