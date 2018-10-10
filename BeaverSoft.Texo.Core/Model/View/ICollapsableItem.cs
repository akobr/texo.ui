using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Model.View
{
    public interface ICollapsableItem : IItem
    {
        string Content { get; }

        bool IsExpanded { get; }
    }
}