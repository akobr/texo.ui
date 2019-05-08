using System;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Transforming
{
    interface ITransformationWithControl<TData> : ITransformation<TData>
    {
        Task<TData> ProcessAsync(TData data, Func<TData, Task<TData>> nextFlow);
    }
}
