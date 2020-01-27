using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core
{
    public interface IAsyncInitialisable
    {
        ValueTask InitialiseAsync();
    }

    public interface IAsyncInitialisable<in TContext>
    {
        ValueTask InitialiseAsync(TContext context);
    }
}
