using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Model.View;

namespace BeaverSoft.Texo.Core.Help
{
    public interface IDidYouMeanService
    {
        IImmutableList<IItem> Help(IInput input);
    }
}