using BeaverSoft.Texo.Core.Configuration;
using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.Input
{
    public interface IInputTreeUpdatedMessage : IServiceMessage
    {
        TextumConfiguration Configuration { get; }

        InputTree.InputTree InputTree { get; }
    }
}
