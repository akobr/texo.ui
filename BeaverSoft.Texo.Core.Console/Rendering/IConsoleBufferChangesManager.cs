namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public interface IConsoleBufferChangesManager
    {
        BufferSequence AllChangeSequence { get; }

        void AddChange(int index);

        void AddChange(int start, int length);

        void Start(int screenStart, int screenLenght, int lineWidth, int cursor);

        ConsoleBufferChangeBatch Finish(ConsoleBufferType batchType, int screenStart, int screenLenght, int lineWidth, int cursor);

        void Resize(int bufferLength);

        void Reset();
    }
}
