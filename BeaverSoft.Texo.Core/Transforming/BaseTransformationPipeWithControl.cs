using System;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Transforming
{
    public abstract class BaseTransformationPipeWithControl<TData> : ITransformationPipeWithControl<TData>
    {
        public abstract Task<TData> ProcessAsync(TData data);

        public virtual async Task<TData> ProcessAsync(TData data, Func<TData, Task<TData>> nextFlow)
        {
            return await nextFlow?.Invoke(await ProcessAsync(data));
        }
    }
}
