using System.Threading;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.Services
{
    public interface IShutdownService
    {
        CancellationToken CancellationToken { get; }

        void Signal();

        void Wait();

        Task DisposeAsync();
    }
}