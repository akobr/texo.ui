using BeaverSoft.Texo.Core.Streaming;
using System.IO;
using System.Text;

namespace Commands.CodeBaseSearch
{
    public class InitialisationReporter : IReporter
    {
        private ReportableStream stream;
        private readonly StreamWriter writer;

        public InitialisationReporter()
        {
            stream = new ReportableStream();
            writer = new StreamWriter(stream.Stream, Encoding.UTF8, 1024, true) { AutoFlush = true };
        }

        public IReportableStream Stream => stream;

        public void Finish()
        {
            if (stream.IsCompleted)
            {
                return;
            }

            stream.NotifyAboutCompletion();
            writer.Close();
        }

        public void Report(string progress)
        {
            if (stream.IsCompleted)
            {
                return;
            }

            writer.Write($"\r{progress}");
        }
    }
}
