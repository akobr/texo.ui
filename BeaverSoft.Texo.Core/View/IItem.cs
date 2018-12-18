using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.View
{
    public interface IItem
    {
        string Text { get; }

        TextFormatEnum Format { get; }

        IImmutableList<IActionLink> Actions { get; }
    }
}