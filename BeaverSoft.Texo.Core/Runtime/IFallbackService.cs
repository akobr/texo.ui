using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Inputting;
using StrongBeaver.Core;

namespace BeaverSoft.Texo.Core.Runtime
{
    public interface IFallbackService : IInitialisable
    {
        Task<ICommandResult> FallbackAsync(Input input, CancellationToken cancellation = default);

        Task<IEnumerable<string>> ProcessIndependentCommandAsync(string input);
    }
}