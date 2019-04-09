using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Runtime
{
    public interface IResultProcessingService
    {
        IImmutableList<IItem> Transfort(ICommandResult result);

        void RegisterMappingService<TContent>(IItemMappingService<TContent> service);

        void UnregisterMappingService<TContent>();
    }
}