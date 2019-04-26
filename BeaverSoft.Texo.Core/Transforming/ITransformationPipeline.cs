using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Transforming
{
    public interface ITransformationPipeline<TData>
    {
        Task<TData> ProcessAsync(TData data);
    }
}
