using System;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Transforming
{
    public interface ITransformationWithControl<TData> : ITransformation<TData>
    {
        Task<TData> ProcessAsync(TData data, Func<TData, Task<TData>> nextFlow);
    }
}
