using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Model.View;

namespace BeaverSoft.Texo.Core.Runtime
{
    public interface IIntellisenceService
    {
        IImmutableList<IItem> Help(IParsedInput input);
    }
}