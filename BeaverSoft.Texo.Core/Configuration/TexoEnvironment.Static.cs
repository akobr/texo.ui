using BeaverSoft.Texo.Core.Environment;

namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class TexoEnvironment
    {
        public static TexoEnvironment Empty { get; } = new TexoEnvironment();

        public static TexoEnvironment CreateDefault()
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
            builder.Variables[VariableNames.CURRENT_DIRECTORY] = System.Environment.CurrentDirectory;
        }
    }
}
