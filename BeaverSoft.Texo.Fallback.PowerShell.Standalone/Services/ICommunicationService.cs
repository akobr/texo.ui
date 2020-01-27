using System.Threading;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.Services
{
    public interface ICommunicationService
    {
        Task StartAsync(CancellationToken cancellationToken = default);
    }
}