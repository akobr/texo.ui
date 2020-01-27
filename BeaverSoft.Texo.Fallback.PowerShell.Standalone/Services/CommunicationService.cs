using System.Threading;
using System.Threading.Tasks;
using BeaverSoft.Texo.Fallback.PowerShell.Core;
using BeaverSoft.Texo.Fallback.PowerShell.Core.Messages;
using BeaverSoft.Texo.Fallback.PowerShell.Standalone.Communication;
using BeaverSoft.Texo.Fallback.PowerShell.Standalone.NamedPipes;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.Services
{
    public class CommunicationService : ICommunicationService
    {
        private readonly ICommunicator communicator;
        private readonly IPowerShellService powerShell;
        private readonly IShutdownService shutdown;

        public CommunicationService(
            ICommunicator communicator,
            IPowerShellService powerShell,
            IShutdownService shutdown)
        {
            this.communicator = communicator;
            this.powerShell = powerShell;
            this.shutdown = shutdown;

            communicator.MessageReceived += OnMessageReceived;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            powerShell.Initialise();
            await communicator.StartAsync(cancellationToken);
        }

        private async void OnMessageReceived(RawMessage message)
        {
            CommandMessage cmdMessage = message.AsType<CommandMessage>();

            switch (cmdMessage.Type)
            {
                case MessageType.RequestCommand:
                    powerShell.AddCommandToQueue(new CommandRequest(cmdMessage.Key, cmdMessage.Content));
                    break;

                case MessageType.RequestCancellation:
                    powerShell.CancelCommand(cmdMessage.Key);
                    await communicator.SendAsync(new CommandMessage() { Key = cmdMessage.Key, Type = MessageType.CommandCancelled });
                    break;

                case MessageType.RequestApplicationShutdown:
                    shutdown.Signal();
                    break;
            }
        }
    }
}
