namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class TexoConfiguration
    {
        public static TexoConfiguration Empty { get; } = new TexoConfiguration();

        public static TexoConfiguration CreateDefault()
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
            TexoRuntime.SetDefault(builder.Runtime);
            TexoEnvironment.SetDefault(builder.Environment);
            TexoUi.SetDefault(builder.Ui);
        }
    }
}
