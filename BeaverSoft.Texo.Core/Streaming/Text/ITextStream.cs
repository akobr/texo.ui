using System;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Streaming.Text
{
    public interface ITextStream : IDisposable
    {
        event EventHandler StreamModified;

        event EventHandler StreamCompleted;

        bool IsClosed { get; }

        string ReadFromBeginningToEnd();

        string ReadLine();

        string ReadToEnd();

        Task<string> ReadFromBeginningToEndAsync();

        Task<string> ReadLineAsync();

        Task<string> ReadToEndAsync();

        void Close();
    }
}