using BeaverSoft.Texo.Core.Configuration;
using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.Inputting
{
    public interface IInputTreeUpdatedMessage : IServiceMessage
    {
        TextumConfiguration Configuration { get; }

        InputTree.InputTree InputTree { get; }
    }
}
