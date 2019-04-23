using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Pipelines
{
    public interface IPipelineUnit<TData>
    {
        Task<TData> ProcessAsync(TData data);
    }
}
