using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Transforming
{
    public interface ITransformation<TData>
    {
        Task<TData> ProcessAsync(TData data);
    }
}
