using System;
using System.IO;

namespace BeaverSoft.Texo.Core.Streaming
{
    public interface IStreamingPipe : IDisposable
    {
        Stream Input { get; }

        Stream Output { get; }

        StreamReader PrepareReader(int bufferSize = 1024, bool leaveOpen = false);

        StreamWriter PrepareWriter(int bufferSize = 1024, bool leaveOpen = false);

        void Restart();
    }
}