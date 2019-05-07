using System.Collections.Generic;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Core.Transforming
{
    public class PipelineFactory<TData>
    {
        private readonly ILogService logger;
        private readonly Dictionary<string, IPipeline<TData>> pipelines;

        public PipelineFactory(ILogService logger)
        {
            this.logger = logger;
            pipelines = new Dictionary<string, IPipeline<TData>>();
        }

        public void Register(string flag, IPipeline<TData> pipeline)
        {
            pipelines[flag] = pipeline;
        }

        public IPipeline<TData> Build(ISet<string> flags)
        {
            foreach (var pipePair in pipelines)
            {
                if (flags.Contains(pipePair.Key))
                {
                    return pipePair.Value;
                }
            }

            return new Pipeline<TData>(logger);
        }
    }
}
