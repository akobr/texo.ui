using BeaverSoft.Texo.Core.Commands;
using StrongBeaver.Core;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Runtime
{
    public interface IFallbackService : IInitialisable
    {
        Task<ICommandResult> FallbackAsync(Input.Input input);
    }
}