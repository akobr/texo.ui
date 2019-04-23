using System;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Pipelines
{
    interface IPipelineUnitWithControl<TData> : IPipelineUnit<TData>
    {
        Task<TData> ProcessAsync(TData data, Func<TData, Task<TData>> nextFlow);
    }
}
