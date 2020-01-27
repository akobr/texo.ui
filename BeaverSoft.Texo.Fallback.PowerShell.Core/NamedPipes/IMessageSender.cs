using System.Threading;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.NamedPipes
{
    public interface IMessageSender
    {
        Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default);

        Task SendTextAsync(string message, CancellationToken cancellationToken = default);
    }
}