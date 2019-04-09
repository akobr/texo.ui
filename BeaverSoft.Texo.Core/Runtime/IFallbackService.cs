using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Core.Runtime
{
    public interface IFallbackService
    {
        ICommandResult Fallback(Input.Input input);
    }
}