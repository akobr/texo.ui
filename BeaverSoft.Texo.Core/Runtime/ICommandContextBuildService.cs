using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Input;

namespace BeaverSoft.Texo.Core.Runtime
{
    public interface ICommandContextBuildService
    {
        ICommandContext BuildContext(IParsedInput input);
    }
}