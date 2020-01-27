using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.NamedPipes
{
    public class NamedPipeServer : INamedPipe
    {
        private readonly NamedPipeServerStream pipe;
        private readonly ILogService logger;

        public NamedPipeServer(ILogService logger)
        {
            pipe = new NamedPipeServerStream(
                Constants.DEFAULT_PIPE_NAME,
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous,
                Constants.DEFAULT_BUFFER_SIZE,
                Constants.DEFAULT_BUFFER_SIZE);

            Messaging = new Messaging(pipe);
            this.logger = logger;
        }

        public bool IsConnected => pipe.IsConnected;

        public IMessaging Messaging { get; }

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            logger.Debug("Waiting for client connection.");
            await pipe.WaitForConnectionAsync(cancellationToken);
            logger.Debug("Secure handshake with the client.");
            await Messaging.SendTextAsync(Constants.SECURITY_PING, cancellationToken);

            string securePong = await Messaging.ReceiveTextAsync(cancellationToken);

            if (!string.Equals(securePong, Constants.SECURITY_PONG, StringComparison.OrdinalIgnoreCase))
            {
                pipe.Close();
                throw new InvalidOperationException("The connection is not secure.");
            }

            logger.Debug("Named pipe connected.");
        }

        public void Dispose()
        {
            pipe.Dispose();
        }
    }
}
