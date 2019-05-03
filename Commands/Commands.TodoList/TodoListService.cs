using System.Collections.Generic;

namespace BeaverSoft.Texo.Commands.TodoList
{
    public class TodoListService : ITodoListService
    {
        private readonly List<TodoItem> items;

        public TodoListService()
        {
            items = new List<TodoItem>();
        }

        public IEnumerable<TodoItem> GetList()
        {
            return items;
        }

        public TodoItem Add(string todo)
        {
            TodoItem item = new TodoItem(todo);
            items.Add(item);
            return item;
        }

        public TodoItem Remove(int index)
        {
            if (index < 0 || index >= items.Count)
            {
                return null;
            }

            TodoItem item = items[index];
            items.RemoveAt(index);
            return item;
        }

        public TodoItem Toggle(int index)
        {
            if (index < 0 || index >= items.Count)
            {
                return null;
            }

            TodoItem item = items[index];
            item.IsFinished = !item.IsFinished;
            return item;
        }
    }
}
