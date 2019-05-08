using System;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Transforming
{
    public abstract class BaseTransformationPipeWithControl<TData> : ITransformationWithControl<TData>
    {
        public abstract Task<TData> ProcessAsync(TData data);

        public virtual async Task<TData> ProcessAsync(TData data, Func<TData, Task<TData>> nextFlow)
        {
            TData transformedData = await ProcessAsync(data);

            if (nextFlow == null)
            {
                return transformedData;
            }

            return await nextFlow(transformedData);
        }
    }
}
