namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class TexoUi
    {
        public const string DEFAULT_PROMPT = "tu";

        public static TexoUi Empty { get; } = new TexoUi();

        public static TexoUi CreateDefault()
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
