using System.Collections.Generic;

namespace BeaverSoft.Texo.Commands.TodoList
{
    public interface ITodoListService
    {
        TodoItem Add(string todo);

        TodoItem Toggle(int index);

        IEnumerable<TodoItem> GetList();

        TodoItem Remove(int index);
    }
}