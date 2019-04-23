using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Pipelines
{
    public interface IPipeline<TData>
    {
        Task<TData> ProcessAsync(TData data);
    }
}
