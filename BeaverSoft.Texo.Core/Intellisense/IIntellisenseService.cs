using System.Collections.Immutable;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Intellisense
{
    public interface IIntellisenseService
    {
        void RegisterExternalHelpProvider(string command, IIntellisenseProvider provider);

        Task<IImmutableList<IItem>> HelpAsync(Input input, int cursorPosition);
    }
}