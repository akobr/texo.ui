using BeaverSoft.Texo.Core.Environment;

namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class TextumEnvironment
    {
        public static TextumEnvironment Empty { get; } = new TextumEnvironment();

        public static TextumEnvironment CreateDefault()
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
