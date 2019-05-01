using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Help
{
    public interface IHelpService
    {
        IImmutableList<IItem> Help(IInput input);
    }
}