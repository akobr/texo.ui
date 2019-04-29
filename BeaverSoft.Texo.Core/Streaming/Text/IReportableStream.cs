using System;
using System.IO;

namespace BeaverSoft.Texo.Core.Streaming.Text
{
    public interface IReportableStream : IDisposable
    {
        event EventHandler StreamModified;

        event EventHandler StreamCompleted;

        bool IsCompleted { get; }

        bool IsClosed { get; }

        Stream Stream { get; }

        string ReadFromBeginningToEnd();
    }
}