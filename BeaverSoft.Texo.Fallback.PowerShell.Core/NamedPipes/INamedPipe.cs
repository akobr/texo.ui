using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.NamedPipes
{
    public interface INamedPipe : IDisposable
    {
        bool IsConnected { get; }

        IMessaging Messaging { get; }

        Task ConnectAsync(CancellationToken cancellationToken = default);
    }
}