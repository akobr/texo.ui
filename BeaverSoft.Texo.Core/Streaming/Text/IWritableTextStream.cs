using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Streaming.Text
{
    public interface IWritableTextStream : ITextStream
    {
        void NotifyAboutChange();

        void NotifyAboutCompletion();

        void Write(string text);

        void WriteLine(string text);

        void WriteLine();

        void Flush();

        Task WriteAsync(string text);

        Task WriteLineAsync(string text);

        Task WriteLineAsync();

        Task FlushAsync();
    }
}