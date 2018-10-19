using System;

namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public partial class TextumRuntime
    {
        public static TextumRuntime Empty { get; } = new TextumRuntime();

        public static TextumRuntime CreateDefault()
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
            builder.Commands.Add(CommandsBuilder.BuildCurrentDirectory());
            builder.Commands.Add(CommandsBuilder.BuildTexo());
            builder.Commands.Add(CommandsBuilder.BuildHelp());

            builder.DefaultCommand = string.Empty;
        }
    }
}