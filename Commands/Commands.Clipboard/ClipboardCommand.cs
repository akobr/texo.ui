using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Extensions;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.Result;
using System;
using System.Collections.Immutable;

using ViewItem = BeaverSoft.Texo.Core.View.Item;

namespace Commands.Clipboard
{
    public class ClipboardCommand : InlineIntersectionCommand
    {
        private readonly ClipboardMonitoringService service;

        public ClipboardCommand(ClipboardMonitoringService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));

            RegisterQueryMethod(ClipboardConstants.QUERY_HISTORY, History);
            RegisterQueryMethod(ClipboardConstants.QUERY_SET, Set);
            RegisterQueryMethod(ClipboardConstants.QUERY_CONFIGURE, Configure);
        }

        private ICommandResult History(CommandContext context)
        {
            if (context.HasOption(ClipboardConstants.OPTION_CLEAR))
            {
                service.ClearHistory();
                return new TextResult("Clipboard history has been cleared.");
            }

            var builder = ImmutableList<ViewItem>.Empty.ToBuilder();

            foreach (string argStrIndex in context.GetParameterValues(ClipboardConstants.PARAMETER_INDEX))
            {
                if (!int.TryParse(argStrIndex, out int argIndex))
                {
                    continue;
                }

                IClipboardItem item = service.GetHistoryItem(argIndex);
                
                if (item == null)
                {
                    continue;
                }

                MarkdownBuilder itemBuilder = new MarkdownBuilder();
                itemBuilder.Header(argIndex.ToString(), 3);
                itemBuilder.CodeBlock(string.Empty, item.Content);
                builder.Add(ViewItem.Markdown(itemBuilder.ToString()));
            }

            if (builder.Count > 0)
            {
                return new ItemsResult(builder.ToImmutable());
            }

            int index = 0;
            foreach (IClipboardItem item in service.GetHistory())
            {
                MarkdownBuilder itemBuilder = new MarkdownBuilder();
                itemBuilder.Header((++index).ToString(), 3);
                itemBuilder.CodeBlock(string.Empty, item.Thumbnail);
                builder.Add(ViewItem.Markdown(itemBuilder.ToString()));
            }

            if (builder.Count <= 0)
            {
                return new TextResult("Clipboard history is empty.");
            }

            return new ItemsResult(builder.ToImmutable());
        }

        private ICommandResult Set(CommandContext context)
        {
            string indexText = context.GetParameterValue(ClipboardConstants.PARAMETER_INDEX);

            if (string.IsNullOrWhiteSpace(indexText))
            {
                indexText = "0";
            }

            if (!int.TryParse(indexText, out int index))
            {
                return new ErrorTextResult("Invalid index.");
            }

            IClipboardItem item = service.SetClipboardFromHistory(service.HistoryCount - index);

            if (item == null)
            {
                return new ErrorTextResult($"An item with index {index} hasn't been found.");
            }

            MarkdownBuilder itemBuilder = new MarkdownBuilder();
            itemBuilder.CodeBlock(string.Empty, item.Content);         
            return new ItemsResult(ViewItem.Markdown(itemBuilder.ToString()));
        }

        private ICommandResult Configure(CommandContext context)
        {
            OptionContext simplify = context.GetOption(ClipboardConstants.OPTION_SIMPLIFY);

            if (simplify != null)
            {
                string value = simplify.GetParameterValue(ClipboardConstants.PARAMETER_FLAG);
                service.Configuration.SimplifyText = string.IsNullOrEmpty(value)
                    ? !service.Configuration.SimplifyText
                    : value.IsTrue();
            }

            OptionContext path = context.GetOption(ClipboardConstants.OPTION_PATH);

            if (path != null)
            {
                string value = path.GetParameterValue(ClipboardConstants.PARAMETER_FLAG);
                service.Configuration.ConvertFilesToPaths = string.IsNullOrEmpty(value)
                    ? !service.Configuration.ConvertFilesToPaths
                    : value.IsTrue();
            }

            MarkdownBuilder itemBuilder = new MarkdownBuilder();
            itemBuilder.WriteLine($"- Text simplification is {EnabledOrDisabled(service.Configuration.SimplifyText)}.");
            itemBuilder.WriteLine($"- File-to-path transfer is {EnabledOrDisabled(service.Configuration.ConvertFilesToPaths)}.");
            return new ItemsResult(ViewItem.Markdown(itemBuilder.ToString()));
        }

        private string EnabledOrDisabled(bool value)
        {
            return value ? "enabled" : "disabled";
        }
    }
}
