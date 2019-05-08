using BeaverSoft.Texo.Core.Configuration;

namespace Commands.Clipboard
{
    public static class ClipboardBuilder
    {
        public static Query BuildCommand()
        {
            var command = Query.CreateBuilder();
            command.Key = ClipboardConstants.COMMAND_NAME;
            command.DefaultQueryKey = ClipboardConstants.QUERY_HISTORY;
            command.Representations.AddRange(
                new[] { ClipboardConstants.COMMAND_NAME, "clip", "cb" });
            command.Documentation.Title = "Clipboard tool";
            command.Documentation.Description = "Simple tool about clipboard with history.";

            var history = Query.CreateBuilder();
            history.Key = ClipboardConstants.QUERY_HISTORY;
            history.Representations.AddRange(
                new[] { ClipboardConstants.QUERY_HISTORY, "list" });
            history.Documentation.Title = "clipboard-history";
            history.Documentation.Description = "Show entire history of the clipboard (maximum of 40 items).";

            var optionClear = Option.CreateBuilder();
            optionClear.Key = ClipboardConstants.OPTION_CLEAR;
            optionClear.Representations.AddRange(
                new[] { ClipboardConstants.OPTION_CLEAR, "c" });
            optionClear.Documentation.Title = ClipboardConstants.OPTION_CLEAR;
            optionClear.Documentation.Description = "Clear entire history.";
            history.Options.Add(optionClear.ToImmutable());

            var parameterIndexes = Parameter.CreateBuilder();
            parameterIndexes.Key = ClipboardConstants.PARAMETER_INDEX;
            parameterIndexes.IsOptional = true;
            parameterIndexes.IsRepeatable = true;
            parameterIndexes.ArgumentTemplate = "\\d*";
            parameterIndexes.Documentation.Title = ClipboardConstants.PARAMETER_INDEX;
            parameterIndexes.Documentation.Description = "Index(es) of item(s) from the history.";
            history.Parameters.Add(parameterIndexes.ToImmutable());

            var set = Query.CreateBuilder();
            set.Key = ClipboardConstants.QUERY_SET;
            set.Representations.AddRange(
                new[] { ClipboardConstants.QUERY_SET, "put", "use" });
            set.Documentation.Title = "clipboard-set";
            set.Documentation.Description = "Set an item to clipboard from history.";

            var parameterIndex = Parameter.CreateBuilder();
            parameterIndex.Key = ClipboardConstants.PARAMETER_INDEX;
            parameterIndex.IsOptional = true;
            parameterIndex.ArgumentTemplate = "\\d*";
            parameterIndex.Documentation.Title = ClipboardConstants.PARAMETER_INDEX;
            parameterIndex.Documentation.Description = "Index of an item from the history.";

            set.Parameters.Add(parameterIndex.ToImmutable());

            var config = Query.CreateBuilder();
            config.Key = ClipboardConstants.QUERY_CONFIGURE;
            config.Representations.AddRange(
                new[] { ClipboardConstants.QUERY_CONFIGURE, "config", "setup" });
            config.Documentation.Title = "clipboard-configure";
            config.Documentation.Description = "Set configuration for automatic clipboard functions.";

            var parameterFlag = Parameter.CreateBuilder();
            parameterFlag.Key = ClipboardConstants.PARAMETER_FLAG;
            parameterFlag.IsOptional = true;
            parameterFlag.ArgumentTemplate = "(true|false|t|f|0|1)";
            parameterFlag.Documentation.Title = ClipboardConstants.PARAMETER_FLAG;
            parameterFlag.Documentation.Description = "The value of a flag.";

            var optionSimplify = Option.CreateBuilder();
            optionSimplify.Key = ClipboardConstants.OPTION_SIMPLIFY;
            optionSimplify.Representations.AddRange(
                new[] { ClipboardConstants.OPTION_SIMPLIFY, "s" });
            optionSimplify.Parameters.Add(parameterFlag.ToImmutable());
            optionSimplify.Documentation.Title = ClipboardConstants.OPTION_SIMPLIFY;
            optionSimplify.Documentation.Description = "Enable or disable automatic text simplification in the clipboard.";
            config.Options.Add(optionSimplify.ToImmutable());

            var optionPath = Option.CreateBuilder();
            optionPath.Key = ClipboardConstants.OPTION_PATH;
            optionPath.Representations.AddRange(
                new[] { ClipboardConstants.OPTION_PATH, "p" });
            optionPath.Parameters.Add(parameterFlag.ToImmutable());
            optionPath.Documentation.Title = ClipboardConstants.OPTION_PATH;
            optionPath.Documentation.Description = "Enable or disable automatic file-to-path transfer in the clipboard.";
            config.Options.Add(optionPath.ToImmutable());

            command.Queries.AddRange(
                new[]
                {
                    history.ToImmutable(),
                    set.ToImmutable(),
                    config.ToImmutable(),
                });

            return command.ToImmutable();
        }
    }
}
