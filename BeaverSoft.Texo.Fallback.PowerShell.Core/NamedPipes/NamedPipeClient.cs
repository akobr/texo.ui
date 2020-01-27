using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.NamedPipes
{
    public class NamedPipeClient : INamedPipe
    {
        private readonly NamedPipeClientStream pipe;
        private readonly ILogService logger;

        public NamedPipeClient(ILogService logger)
        {
            pipe = new NamedPipeClientStream(
                Constants.LOCAL_SERVER_NAME,
                Constants.DEFAULT_PIPE_NAME,
                PipeDirection.InOut,
                PipeOptions.Asynchronous,
                TokenImpersonationLevel.None,
                HandleInheritability.None);

            Messaging = new Messaging(pipe);
            this.logger = logger;
        }

        public bool IsConnected => pipe.IsConnected;

        public IMessaging Messaging { get; }

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            logger.Debug("Connecting to server.");
            await pipe.ConnectAsync(cancellationToken);
            pipe.ReadMode = PipeTransmissionMode.Message;
            logger.Debug("Secure handshake with the server.");
            string securityPing = await Messaging.ReceiveTextAsync(cancellationToken);

            if (!string.Equals(securityPing, Constants.SECURITY_PING, StringComparison.OrdinalIgnoreCase))
            {
                pipe.Close();
                throw new InvalidOperationException("The connection is not secure.");
            }

            await Messaging.SendTextAsync(Constants.SECURITY_PONG, cancellationToken);
            logger.Debug("Named pipe connected.");
        }

        public void Dispose()
        {
            pipe.Dispose();
        }
    }
}
