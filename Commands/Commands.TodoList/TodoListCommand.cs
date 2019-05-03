using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Extensibility.Attributes;
using BeaverSoft.Texo.Core.Result;
using System;

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
            return new ErrorTextResult("Not implemented (add).");
        }

        [Query("remove", Representations = "remove delete r d")]
        [Parameter("index", IsRepetable = true, ParameterTemplate = "\\d+")]
        [Documentation("Remove task", "Removes a task or tasks from list.")]
        public ICommandResult Remove(CommandContext context)
        {
            return new ErrorTextResult("Not implemented (remove).");
        }

        [Query("toggle", Representations = "toggle change state finish t s f")]
        [Parameter("index", IsRepetable = true, ParameterTemplate = "\\d+")]
        [Documentation("Remove task", "Removes a task or tasks from list.")]
        public ICommandResult Toggle(CommandContext context)
        {
            return new ErrorTextResult("Not implemented (toggle).");
        }
    }
}
