using System;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public interface IConsoleBufferBuilder : IDisposable
    {
        void Start();

        ConsoleBuffer Snapshot(ConsoleBufferType type = ConsoleBufferType.Screen);
    }
}