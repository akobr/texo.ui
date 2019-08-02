using System.Collections.Immutable;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Runtime
{
    public interface IItemMappingService
    {
        // no member
    }

    public interface IItemMappingService<in TContent> : IItemMappingService
    {
        IImmutableList<IItem> Map(TContent content);
    }
}