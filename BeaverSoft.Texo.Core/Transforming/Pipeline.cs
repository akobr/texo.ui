using StrongBeaver.Core.Services.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Transforming
{
    public class Pipeline<TData> : IPipeline<TData>
    {
        protected readonly ILogService logger;
        private readonly LinkedList<ITransformation<TData>> pipeline;

        public Pipeline(ILogService logger)
        {
            this.logger = logger;
            pipeline = new LinkedList<ITransformation<TData>>();
        }

        public void AddPipe(ITransformation<TData> pipe)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }

            pipeline.AddLast(pipe);
        }

        public bool AddPipeBefore(ITransformation<TData> pipeToAdd, ITransformation<TData> pipeReference)
        {
            if (pipeToAdd == null)
            {
                throw new ArgumentNullException(nameof(pipeToAdd));
            }

            if (pipeReference == null)
            {
                throw new ArgumentNullException(nameof(pipeReference));
            }

            var unitNode = pipeline.Find(pipeReference);

            if (unitNode == null)
            {
                return false;
            }

            pipeline.AddBefore(unitNode, pipeToAdd);
            return true;
        }

        public async Task<TData> ProcessAsync(TData data)
        {
            return await OnPipelineProcessed(await ProcessUnitAsync(pipeline.First, data));
        }

        protected virtual void OnProcessingPipeNode(LinkedListNode<ITransformation<TData>> pipeNode, TData data)
        {
            // no operation ( template method )
        }

        protected virtual Task<TData> OnPipelineProcessed(TData data)
        {
            // template method
            return Task.FromResult(data);
        }

        private Task<TData> ProcessUnitAsync(LinkedListNode<ITransformation<TData>> pipeNode, TData data)
        {
            if (pipeNode == null)
            {
                return Task.FromResult(data);
            }

            OnProcessingPipeNode(pipeNode, data);

            if (pipeNode.Value is ITransformationWithControl<TData> controlPipe)
            {
                return ProcessControlUnitAsync(pipeNode, controlPipe, data);
            }

            return ProcessSimpleUnitAsync(pipeNode, data);
        }

        private async Task<TData> ProcessSimpleUnitAsync(LinkedListNode<ITransformation<TData>> pipeNode, TData data)
        {
            try
            {
                data = await pipeNode.Value.ProcessAsync(data);
            }
            catch (Exception exception)
            {
                logger.Error($"Error during processing pipeline of '{typeof(TData).Name}' in unit '{pipeNode.Value.GetType().Name}'.", exception);
            }

            return await ProcessUnitAsync(pipeNode.Next, data);
        }

        private Task<TData> ProcessControlUnitAsync(
            LinkedListNode<ITransformation<TData>> pipeNode,
            ITransformationWithControl<TData> controlPipe,
            TData data)
        {
            try
            {
                return controlPipe.ProcessAsync(data, (resultData) => ProcessUnitAsync(pipeNode.Next, resultData));
            }
            catch (Exception exception)
            {
                logger.Error(
                    $"Error during processing pipeline of '{typeof(TData).Name}' in unit '{pipeNode.Value.GetType().Name}'.",
                    exception,
                    "The unit with control of the flow will be skipped.");

                // Skip of the unit
                return ProcessUnitAsync(pipeNode.Next, data);
            }
        }
    }
}
