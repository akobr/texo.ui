namespace BeaverSoft.Texo.Core.Commands
{
    public partial class CommandContext
    {
        public static CommandContext ShiftQuery(CommandContext context)
        {
            if (context == null
                || !context.IsValid
                || context.QueryPath.Count <= 1)
            {
                return context;
            }

            Builder builder = context.ToBuilder();
            builder.QueryPath.RemoveAt(0);
            builder.Key = context.QueryPath[0];
            return builder.ToImmutable();
        }
    }
}