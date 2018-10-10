namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public partial class TextumUi
    {
        public const string DEFAULT_PROMPT = "tu";

        public static TextumUi Empty { get; } = new TextumUi();

        public static TextumUi CreateDefault()
        {
            return Empty.SetPrompt(DEFAULT_PROMPT);
        }

        public static Builder CreateBuilder()
        {
            return Empty.ToBuilder();
        }
    }
}
