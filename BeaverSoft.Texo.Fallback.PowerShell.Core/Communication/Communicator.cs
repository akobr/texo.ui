using System;
using System.Threading;
using System.Threading.Tasks;
using BeaverSoft.Texo.Fallback.PowerShell.Standalone.NamedPipes;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.Communication
{
    public class Communicator : ICommunicator
    {
        private readonly CancellationTokenSource cancelationSource;

        public event Action<RawMessage> MessageReceived;

        public Communicator(INamedPipe pipe)
        {
            Pipe = pipe ?? throw new ArgumentNullException(nameof(pipe));
            cancelationSource = new CancellationTokenSource();
        }

        public INamedPipe Pipe { get; }

        public Task Receiving { get; private set; }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (Pipe.IsConnected)
            {
                return;
            }

            await Pipe.ConnectAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            Receiving = Task.Run(() => ReceivingAsync(cancelationSource.Token), cancelationSource.Token);
        }

        public Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        {
            return Pipe.Messaging.SendAsync(message, cancellationToken);
        }

        public void Dispose()
        {
            cancelationSource.Cancel();
            Pipe.Dispose();
        }

        private async Task ReceivingAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                RawMessage message = await Pipe.Messaging.ReceiveAsync(cancellationToken);

                if (message.Data.IsEmpty)
                {
                    break;
                }

                MessageReceived?.Invoke(message);
            }
        }
    }
}
