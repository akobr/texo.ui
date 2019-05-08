using System;
using System.IO;

namespace BeaverSoft.Texo.Core.Streaming
{
    public class ConcurrentMemoryStream : Stream
    {
        private readonly MemoryStream innerStream;
        private long writePosition;
        private long readPosition;
        private Action onFlush;

        public ConcurrentMemoryStream(Action onFlush)
        {
            innerStream = new MemoryStream();
            this.onFlush = onFlush;
        }

        public ConcurrentMemoryStream()
            : this(null)
        {
            // no operation
        }


        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length
        {
            get
            {
                lock (innerStream)
                {
                    return innerStream.Length;
                }
            }
        }

        [Obsolete("Always gets and sets a position for writing! The WritingPosition and ReadingPosition properties should be used instead.")]
        public override long Position
        {
            get => WritingPosition;
            set => WritingPosition = value;
        }

        public long WritingPosition
        {
            get
            {
                lock (innerStream)
                {
                    return writePosition;
                }
            }

            set
            {
                lock (innerStream)
                {
                    innerStream.Position = value;
                    writePosition = innerStream.Position;
                }
            }
        }

        public long ReadingPosition
        {
            get
            {
                lock (innerStream)
                {
                    return readPosition;
                }
            }

            set
            {
                lock (innerStream)
                {
                    innerStream.Position = value;
                    readPosition = innerStream.Position;
                }
            }
        }

        public override void Flush()
        {
            lock (innerStream)
            {
                innerStream.Flush();
                onFlush?.Invoke();
            }
        }

        public override void Close()
        {
            lock (innerStream)
            {
                innerStream.Close();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                innerStream.Dispose();
            }

            base.Dispose(disposing);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (innerStream)
            {
                innerStream.Position = readPosition;
                int readed = innerStream.Read(buffer, offset, count);
                readPosition = innerStream.Position;

                return readed;
            }
        }

        [Obsolete("Seek will always set a new position for writing! The SeekWriting or SeekReading method should be used instead.")]
        public override long Seek(long offset, SeekOrigin origin)
        {
            return SeekWriting(offset, origin);
        }

        public long SeekWriting(long offset, SeekOrigin origin)
        {
            lock (this)
            {
                innerStream.Position = writePosition;
                writePosition = innerStream.Seek(offset, origin);
                return writePosition;
            }
        }

        public long SeekReading(long offset, SeekOrigin origin)
        {
            lock (this)
            {
                innerStream.Position = readPosition;
                readPosition = innerStream.Seek(offset, origin);
                return readPosition;
            }
        }

        public override void SetLength(long value)
        {
            lock (innerStream)
            {
                innerStream.SetLength(value);
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (innerStream)
            {
                innerStream.Position = writePosition;
                innerStream.Write(buffer, offset, count);
                writePosition = innerStream.Position;
            }
        }
    }
}
