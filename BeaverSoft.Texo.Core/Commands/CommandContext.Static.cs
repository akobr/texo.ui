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

            return ShiftQuery(context, context.QueryPath[0]);
        }

        public static CommandContext ShiftQuery(CommandContext context, string queryKey)
        {
            if (context == null
                || !context.IsValid
                || context.QueryPath.Count <= 1)
            {
                return context;
            }

            Builder builder = context.ToBuilder();
            builder.QueryPath.RemoveAt(0);
            builder.Key = queryKey;
            return builder.ToImmutable();
        }
    }
}