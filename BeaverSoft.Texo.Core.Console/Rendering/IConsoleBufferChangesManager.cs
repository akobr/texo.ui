namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public interface IConsoleBufferChangesManager
    {
        void AddChange(int index);

        void AddChange(int start, int length);

        void Start(int screenStart, int screenLenght, int lineWidth, int cursor);

        ConsoleBufferChangeBatch Finish(int screenStart, int screenLenght, int lineWidth, int cursor);

        void Reset();
    }
}
