using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Transforming
{
    public interface ITransformationPipe<TData>
    {
        Task<TData> ProcessAsync(TData data);
    }
}
