using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Extensibility.Attributes;
using BeaverSoft.Texo.Core.Result;
using System;
using System.Collections.Generic;

[assembly: CommandLibrary]

namespace BeaverSoft.Texo.Commands.TodoList
{
    [Command("todo-list", Representations = "todo-list todo tl")]
    [Documentation("Todo list", "Simple markdown-based todo list!")]
    public class TodoListCommand
    {
        private readonly ITodoListService service;

        public TodoListCommand(ITodoListService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [Query("add", Representations = "add new task a n")]
        [Parameter("todo", IsRepetable = true)]
        [Documentation("Add task", "Adds a task or tasks to list.")]
        public ICommandResult Add(CommandContext context)
        {
            List<TodoItem> newItems = new List<TodoItem>();

            foreach (string todoItem in context.GetParameterValues("todo"))
            {
                newItems.Add(service.Add(todoItem));
            }

            return ShowQuery.BuildMarkdownTaskListResult(newItems);
        }

        [Query("remove", Representations = "remove delete r d")]
        [Parameter("index", IsRepetable = true, ParameterTemplate = "\\d+")]
        [Documentation("Remove task", "Removes a task or tasks from list.")]
        public ICommandResult Remove(CommandContext context)
        {
            int count = 0;

            foreach (string indexValue in context.GetParameterValues("index"))
            {
                if (!int.TryParse(indexValue, out int index))
                {
                    continue;
                }

                if (service.Remove(index) != null)
                {
                    count++;
                }
            }

            return new TextResult($"{count} item(s) removed.");
        }

        [Query("toggle", Representations = "toggle change state finish t s f")]
        [Parameter("index", IsRepetable = true, ParameterTemplate = "\\d+")]
        [Documentation("Toggle task", "Makes a task finished / unfinished.")]
        public ICommandResult Toggle(CommandContext context)
        {
            List<TodoItem> toggledItems = new List<TodoItem>();

            foreach (string indexValue in context.GetParameterValues("index"))
            {
                if (!int.TryParse(indexValue, out int index))
                {
                    continue;
                }

                TodoItem item = service.Toggle(index);

                if (item == null)
                {
                    continue;
                }

                toggledItems.Add(item);
            }

            return ShowQuery.BuildMarkdownTaskListResult(toggledItems);
        }
    }
}
