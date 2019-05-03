using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Extensibility.Attributes;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;
using System;

namespace BeaverSoft.Texo.Commands.TodoList
{
    [Query("show", Representations = "show list display" , Path = "todo-list")]
    [Option("open-only", Representations = "open-only open o")]
    [Documentation("List of todos", "Displays list of all todo items.")]
    [Documentation("Open todos only", "With this option gonna show only open items.", Path = "todo-list show open-only" )]
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
            IMarkdownBuilder builder = new MarkdownBuilder();

            foreach (TodoItem item in service.GetList())
            {
                if (openOnly && item.IsFinished)
                {
                    continue;
                }

                builder.Bullet($"{GetTaskPrefix(item)} {item.Text}");
            }

            return new ItemsResult(Item.Markdown(builder.ToString()));
        }

        private string GetTaskPrefix(TodoItem item)
        {
            return item.IsFinished ? "[x]" : "[ ]";
        }
    }
}
