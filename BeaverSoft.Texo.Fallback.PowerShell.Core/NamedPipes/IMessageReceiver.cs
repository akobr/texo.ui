using System.Threading;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.NamedPipes
{
    public interface IMessageReceiver
    {
        ValueTask<RawMessage> ReceiveAsync(CancellationToken cancellationToken = default);

        ValueTask<TMessage> ReceiveAsync<TMessage>(CancellationToken cancellationToken = default);

        ValueTask<string> ReceiveTextAsync(CancellationToken cancellationToken = default);
    }
}