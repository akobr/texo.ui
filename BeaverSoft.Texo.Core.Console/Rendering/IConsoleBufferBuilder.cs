namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public interface IConsoleBufferBuilder
    {
        void Start();

        ConsoleBuffer Snapshot();
    }
}