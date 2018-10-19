namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public partial class TextumUi
    {
        public const string DEFAULT_PROMPT = "tu";

        public static TextumUi Empty { get; } = new TextumUi();

        public static TextumUi CreateDefault()
        {
            Builder builder = CreateBuilder();
            SetDefault(builder);
            return builder.ToImmutable();
        }

        public static Builder CreateBuilder()
        {
            return Empty.ToBuilder();
        }

        internal static void SetDefault(Builder builder)
        {
            builder.Prompt = DEFAULT_PROMPT;
        }
    }
}
