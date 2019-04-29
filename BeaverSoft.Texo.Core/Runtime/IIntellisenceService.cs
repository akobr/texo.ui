using System.Collections.Immutable;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Runtime
{
    public interface IIntellisenceService
    {
        Task<IImmutableList<IItem>> HelpAsync(Input.Input input, int cursorPosition);
    }
}