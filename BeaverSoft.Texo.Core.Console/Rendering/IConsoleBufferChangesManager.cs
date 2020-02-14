namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public interface IConsoleBufferChangesManager
    {
        BufferSequence AllChangeSequence { get; }

        void AddChange(int index);

        void AddChange(int start, int length);

        void Start(SizedBufferSequence screen, int cursor);

        ConsoleBufferChangeBatch Finish(SizedBufferSequence screen, int cursor, int snapshotStartIndex, int snapshotLength);

        void Resize(int bufferLength);

        void Reset();
    }
}
