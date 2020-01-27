using System;
using System.IO;
using System.Text;

namespace BeaverSoft.Texo.Core.Streaming
{
    public class ReportableStream : IReportableStream, IFinishableReportableStream, IDisposable
    {
        private readonly ConcurrentMemoryStream stream;

        public ReportableStream()
        {
            stream = new ConcurrentMemoryStream(NotifyAboutModification);
        }

        public event EventHandler StreamModified;

        public event EventHandler StreamCompleted;

        public bool IsCompleted { get; private set; }

        public bool IsClosed => !stream.CanRead;

        public Stream Stream => stream;

        public void NotifyAboutCompletion()
        {
            if (IsCompleted)
            {
                return;
            }

            IsCompleted = true;
            StreamCompleted?.Invoke(this, new EventArgs());
        }

        public void NotifyAboutModification()
        {
            StreamModified?.Invoke(this, new EventArgs());
        }

        public string ReadFromBeginningToEnd()
        {
            using (TextReader reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true))
            {
                long currentPosition = stream.ReadingPosition;
                stream.SeekReading(0, SeekOrigin.Begin);
                return reader.ReadToEnd();
            }
        }

        public void Dispose()
        {
            stream.Dispose();
        }
    }
}
