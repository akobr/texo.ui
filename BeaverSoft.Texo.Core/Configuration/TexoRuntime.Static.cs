namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class TexoRuntime
    {
        public static TexoRuntime Empty { get; } = new TexoRuntime();

        public static TexoRuntime CreateDefault()
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
            builder.Commands.Clear();
            builder.Commands.Add(CommandsBuilder.BuildClear());
            builder.Commands.Add(CommandsBuilder.BuildCurrentDirectory());
            builder.Commands.Add(CommandsBuilder.BuildTexo());
            builder.Commands.Add(CommandsBuilder.BuildHelp());

            builder.DefaultCommand = string.Empty;
        }
    }
}