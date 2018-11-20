using System.Collections.Immutable;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Runtime
{
    public interface IIntellisenceService
    {
        IImmutableList<IItem> Help(Input.Input input, int cursorPosition);
    }
}