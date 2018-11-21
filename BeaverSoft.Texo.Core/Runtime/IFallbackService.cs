using BeaverSoft.Texo.Core.Commands;
using StrongBeaver.Core;

namespace BeaverSoft.Texo.Core.Runtime
{
    public interface IFallbackService : IInitialisable
    {
        ICommandResult Fallback(Input.Input input);
    }
}