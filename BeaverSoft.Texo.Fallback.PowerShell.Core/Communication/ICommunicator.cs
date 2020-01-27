using System;
using System.Threading;
using System.Threading.Tasks;
using BeaverSoft.Texo.Fallback.PowerShell.Standalone.NamedPipes;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.Communication
{
    public interface ICommunicator : IDisposable
    {
        INamedPipe Pipe { get; }

        Task Receiving { get; }

        event Action<RawMessage> MessageReceived;

        Task StartAsync(CancellationToken cancellationToken = default);

        Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default);
    }
}