using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Transforming;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Fallback.PowerShell.Transforming
{
    public abstract class BaseCommandOutput : ITransformationWithControl<OutputModel>
    {
        private readonly ILogService logger;

        public BaseCommandOutput(ILogService logger)
        {
            this.logger = logger;
            Pipeline = new Pipeline<OutputModel>(logger);
        }

        protected ILogService Logger => logger;

        protected IPipeline<OutputModel> Pipeline { get; }

        public async Task<OutputModel> ProcessAsync(OutputModel data, Func<OutputModel, Task<OutputModel>> nextFlow)
        {
            if (!IsInterestedOutput(data))
            {
                return await nextFlow(data);
            }

            return await ProcessAsync(data);
        }

        protected abstract bool IsInterestedOutput(OutputModel data);

        public virtual Task<OutputModel> ProcessAsync(OutputModel data)
        {
            return Pipeline.ProcessAsync(data);
        }
    }
}
