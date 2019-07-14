using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Extensibility.Attributes;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;
using System;
using System.Collections.Generic;

namespace BeaverSoft.Texo.Commands.TodoList
{
    [SubCommand("show", Representations = "show list display" , Path = "todo-list")]
    [Option("open-only", Representations = "open-only open o")] 
    [Documentation("List of todos", "Displays list of all todo items.")]
    [Documentation("Open todos only", "With this option gonna show only open items.", Path = "open-only" )]

    // DELETE: just the example of complex usage of attributes 
    [Parameter("index", ParameterTemplate = "^[0-9]+$")]
    [Parameter("index", OptionKey = "open-only", ParameterTemplate = "^[0-9]+$")]
    [Documentation("Test index parameter", "Test description.", Path = "o:open-only.index")]
    [LocalisedDocumentation("PAR_INDEX_NAME", "PAR_INDEX_DESC", Path = "p:index")]

    // representations can be separated by space or comma
    // prefix p:(parameter) or o:(option) is neccesary only when there name/key collision
    // path is always starting in current context (where the command's path ends)
    // path can be separeted by space or dot

    public class ShowQuery : ICommand
    {
        private readonly ITodoListService service;

        public ShowQuery(ITodoListService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public ICommandResult Execute(CommandContext context)
        {
            bool openOnly = context.HasOption("open-only");
            return BuildMarkdownTaskListResult(service.GetList(), openOnly);
        }

        internal static ICommandResult BuildMarkdownTaskListResult(IEnumerable<TodoItem> items, bool openOnly = false)
        {
            IMarkdownBuilder builder = new MarkdownBuilder();

            foreach (TodoItem item in items)
            {
                if (openOnly && item.IsFinished)
                {
                    continue;
                }

                builder.Bullet($"{GetMarkdownTaskPrefix(item)} {item.Text}");
            }

            return new ItemsResult(Item.AsMarkdown(builder.ToString()));
        }

        private static string GetMarkdownTaskPrefix(TodoItem item)
        {
            return item.IsFinished ? "[x]" : "[ ]";
        }
    }
}
