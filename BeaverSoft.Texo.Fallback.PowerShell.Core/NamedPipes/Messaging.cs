using System;
using System.Buffers;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.NamedPipes
{
    public class Messaging : IMessaging
    {
        private readonly PipeStream pipe;
        private readonly IMessageEncoding enconding;

        public Messaging(PipeStream pipe)
            : this(pipe, new Utf8MessageEncoding(null)) { }

        public Messaging(PipeStream pipe, IMessageEncoding enconding)
        {
            this.pipe = pipe ?? throw new ArgumentNullException(nameof(pipe));
            this.enconding = enconding ?? throw new ArgumentNullException(nameof(enconding));
        }

        public Task SendTextAsync(string message, CancellationToken cancellationToken = default)
        {
            byte[] bytes = enconding.Encode(message);
            return pipe.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
        }

        public Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        {
            byte[] bytes = enconding.Encode(message);
            return pipe.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
        }

        public ValueTask<RawMessage> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            return ReceiveAsync(
                (mem) => new RawMessage(mem, enconding),
                cancellationToken);
        }

        public ValueTask<string> ReceiveTextAsync(CancellationToken cancellationToken = default)
        {
            return ReceiveAsync(
                (mem) => enconding.Decode(mem.Span),
                cancellationToken);
        }

        public ValueTask<TMessage> ReceiveAsync<TMessage>(CancellationToken cancellationToken = default)
        {
            return ReceiveAsync(
                (mem) => enconding.Decode<TMessage>(mem.Span),
                cancellationToken);
        }

        private async ValueTask<TOutput> ReceiveAsync<TOutput>(
            Func<Memory<byte>, TOutput> transform,
            CancellationToken cancellationToken)
        {
            int index = 0;
            var buffer = ArrayPool<byte>.Shared.Rent(Constants.DEFAULT_BUFFER_SIZE);

            try
            {
                do
                {
                    int readed = await pipe.ReadAsync(buffer, index, buffer.Length - index, cancellationToken);
                    index += readed;

                    if (pipe.IsMessageComplete || readed == 0)
                    {
                        break;
                    }

                    if (index > buffer.Length * 0.85)
                    {
                        var bufferTemp = buffer;
                        buffer = ArrayPool<byte>.Shared.Rent(buffer.Length * 2);
                        Array.Copy(bufferTemp, 0, buffer, 0, index);
                        ArrayPool<byte>.Shared.Return(bufferTemp);
                    }

                } while (!pipe.IsMessageComplete);

                return transform(buffer.AsMemory(0, index));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        // TODO: [P3] use this more robuse version or reading
        //private async ValueTask<TOutput> ReceiveAsync<TOutput>(
        //    Func<PipeReader, ValueTask<TOutput>> transform,
        //    CancellationToken cancellationToken)
        //{
        //    Pipe localPipe = new Pipe();

        //    do
        //    {
        //        Memory<byte> buffer = localPipe.Writer.GetMemory(Constants.DEFAULT_BUFFER_SIZE);
        //        int readed = await pipe.ReadAsync(buffer, cancellationToken);

        //        if (readed == 0)
        //        {
        //            break;
        //        }

        //        localPipe.Writer.Advance(readed);
        //    } while (!pipe.IsMessageComplete);

        //    localPipe.Writer.Complete();
        //    TOutput result = await transform(localPipe.Reader);
        //    localPipe.Reader.Complete();
        //    return result;
        //}
    }
}
