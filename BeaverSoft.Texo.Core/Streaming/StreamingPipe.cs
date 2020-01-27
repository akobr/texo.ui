using System.IO;
using System.IO.Pipelines;
using System.Text;

namespace BeaverSoft.Texo.Core.Streaming
{
    public class StreamingPipe : IStreamingPipe
    {
        private readonly Pipe pipe;
        private readonly Encoding encoding;

        public StreamingPipe()
            : this(Encoding.UTF8) { }

        public StreamingPipe(Encoding encoding)
        {
            this.encoding = encoding;

            pipe = new Pipe();
            PrepareStreams();
        }

        public Stream Input { get; private set; }

        public Stream Output { get; private set; }

        public StreamReader PrepareReader(int bufferSize = 1024, bool leaveOpen = false)
        {
            return new StreamReader(Output, encoding, false, bufferSize, leaveOpen);
        }

        public StreamWriter PrepareWriter(int bufferSize = 1024, bool leaveOpen = false)
        {
            return new StreamWriter(Input, encoding, bufferSize, leaveOpen);
        }

        public void Restart()
        {
            DisposeStreams();
            pipe.Reset();
            PrepareStreams();
        }

        public void Dispose()
        {
            DisposeStreams();
        }

        private void PrepareStreams()
        {
            Input = pipe.Writer.AsStream();
            Output = pipe.Reader.AsStream();
        }

        private void DisposeStreams()
        {
            Input.Dispose();
            Output.Dispose();
        }
    }
}
