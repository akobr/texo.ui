using System;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Transforming
{
    interface ITransformationPipeWithControl<TData> : ITransformationPipe<TData>
    {
        Task<TData> ProcessAsync(TData data, Func<TData, Task<TData>> nextFlow);
    }
}
