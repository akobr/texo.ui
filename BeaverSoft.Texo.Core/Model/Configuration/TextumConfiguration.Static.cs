namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public partial class TextumConfiguration
    {
        public static TextumConfiguration Empty { get; } = new TextumConfiguration();

        public static TextumConfiguration CreateDefault()
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
            TextumRuntime.SetDefault(builder.Runtime);
            TextumEnvironment.SetDefault(builder.Environment);
            TextumUi.SetDefault(builder.Ui);
        }
    }
}
