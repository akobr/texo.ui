namespace BeaverSoft.Texo.Commands.TodoList
{
    public class TodoItem
    {
        public TodoItem(string todo)
        {
            Text = todo;
        }

        public string Text { get; set; }

        public bool IsFinished { get; set; }
    }
}
