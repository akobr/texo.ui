using System;
using System.Threading;
using System.Threading.Tasks;
using BeaverSoft.Texo.Fallback.PowerShell.Core.Messages;
using BeaverSoft.Texo.Fallback.PowerShell.Standalone.Communication;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.Services
{
    public class ShutdownService : IShutdownService
    {
        private readonly ICommunicator communicator;
        private readonly IPowerShellService powerShell;
        private readonly ManualResetEventSlim waitHandler;
        private readonly CancellationTokenSource cancellationSource;

        public ShutdownService(ICommunicator communicator, IPowerShellService powerShell)
        {
            this.communicator = communicator;
            this.powerShell = powerShell;

            waitHandler = new ManualResetEventSlim();
            cancellationSource = BuildCancellationSource();
            CancellationToken = cancellationSource.Token;
        }

        public CancellationToken CancellationToken { get; }

        public void Signal()
        {
            cancellationSource.Cancel();
            waitHandler.Set();
        }

        public void Wait()
        {
            waitHandler.Wait(cancellationSource.Token);
        }

        public async Task DisposeAsync()
        {
            powerShell.Dispose();
            await Task.Delay(200);

            await communicator.SendAsync(new CommandMessage() { Key = Guid.Empty, Type = MessageType.ApplicationShutdown });

            communicator.Dispose();
            waitHandler.Dispose();
            await Task.Delay(50);
        }

        private static CancellationTokenSource BuildCancellationSource()
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            return cts;
        }
    }
}
