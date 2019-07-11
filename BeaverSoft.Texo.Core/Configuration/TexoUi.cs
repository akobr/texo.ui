namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class TexoUi
    {
        private string prompt;
        private bool showWorkingPathAsPrompt;

        private TexoUi()
        {
            prompt = string.Empty;
            showWorkingPathAsPrompt = false;
        }

        private TexoUi(TexoUi toClone)
        {
            prompt = toClone.prompt;
            showWorkingPathAsPrompt = toClone.showWorkingPathAsPrompt;
        }

        private TexoUi(Builder builder)
        {
            prompt = builder.Prompt;
            showWorkingPathAsPrompt = builder.ShowWorkingPathAsPrompt;
        }

        public string Prompt => prompt;

        public bool ShowWorkingPathAsPrompt => showWorkingPathAsPrompt;

        public TexoUi SetPrompt(string value)
        {
            return new TexoUi(this)
            {
                prompt = value
            };
        }

        public TexoUi SetShowWorkingPathAsPrompt(bool value)
        {
            return new TexoUi(this)
            {
                showWorkingPathAsPrompt = value
            };
        }

        public Builder ToBuilder()
        {
            return new Builder(this);
        }
    }
}
