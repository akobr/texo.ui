using System.Collections.Immutable;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Runtime
{
    public interface IResultProcessingService
    {
        Task<IImmutableList<IItem>> TransfortAsync(ICommandResult result);

        void RegisterMappingService<TContent>(IItemMappingService<TContent> service);

        void UnregisterMappingService<TContent>();
    }
}