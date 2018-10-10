using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Model.View;

namespace BeaverSoft.Texo.Core.Runtime
{
    public interface IDidYouMeanService
    {
        IImmutableList<IItem> Help(IParsedInput input);
    }
}