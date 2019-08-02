using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Inputting.Tree;
using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.Inputting
{
    public interface IInputTreeUpdatedMessage : IServiceMessage
    {
        TexoConfiguration Configuration { get; }

        InputTree InputTree { get; }
    }
}
