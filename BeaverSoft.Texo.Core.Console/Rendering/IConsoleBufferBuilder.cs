using System;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public interface IConsoleBufferBuilder : IDisposable
    {
        void Start();

        ConsoleBuffer Snapshot(ConsoleBufferType bufferType = ConsoleBufferType.Screen);
    }
}