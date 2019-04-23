using StrongBeaver.Core.Services.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Pipelines
{
    public class Pipeline<TData> : IPipeline<TData>
    {
        private readonly ILogService logger;
        private readonly LinkedList<IPipelineUnit<TData>> pipe;

        public Pipeline(ILogService logger)
        {
            this.logger = logger;
            pipe = new LinkedList<IPipelineUnit<TData>>();
        }

        public void AddUnit(IPipelineUnit<TData> pipeUnit)
        {
            if (pipeUnit == null)
            {
                throw new ArgumentNullException(nameof(pipeUnit));
            }

            pipe.AddLast(pipeUnit);
        }

        public bool AddUnitBefore(IPipelineUnit<TData> pipeUnitToAdd, IPipelineUnit<TData> pipeUnitReference)
        {
            if (pipeUnitToAdd == null)
            {
                throw new ArgumentNullException(nameof(pipeUnitToAdd));
            }

            if (pipeUnitReference == null)
            {
                throw new ArgumentNullException(nameof(pipeUnitReference));
            }

            var unitNode = pipe.Find(pipeUnitReference);

            if (unitNode == null)
            {
                return false;
            }


            pipe.AddBefore(unitNode, pipeUnitToAdd);
            return true;
        }

        public Task<TData> ProcessAsync(TData data)
        {
            return ProcessUnitAsync(pipe.First, data);
        }

        private Task<TData> ProcessUnitAsync(LinkedListNode<IPipelineUnit<TData>> unitNode, TData data)
        {
            if (unitNode == null)
            {
                return Task.FromResult(data);
            }

            if (unitNode.Value is IPipelineUnitWithControl<TData> controlUnit)
            {
                return ProcessControlUnitAsync(unitNode, controlUnit, data);
            }

            return ProcessSimpleUnitAsync(unitNode, data);
        }

        private async Task<TData> ProcessSimpleUnitAsync(LinkedListNode<IPipelineUnit<TData>> unitNode, TData data)
        {
            try
            {
                data = await unitNode.Value.ProcessAsync(data);
            }
            catch (Exception exception)
            {
                logger.Error($"Error during processing pipeline of '{typeof(TData).Name}' in unit '{unitNode.Value.GetType().Name}'.", exception);
            }

            return await ProcessUnitAsync(unitNode.Next, data);
        }

        private Task<TData> ProcessControlUnitAsync(
            LinkedListNode<IPipelineUnit<TData>> unitNode,
            IPipelineUnitWithControl<TData> controlUnit,
            TData data)
        {
            try
            {
                return controlUnit.ProcessAsync(data, (resultData) => ProcessUnitAsync(unitNode.Next, resultData));
            }
            catch (Exception exception)
            {
                logger.Error(
                    $"Error during processing pipeline of '{typeof(TData).Name}' in unit '{unitNode.Value.GetType().Name}'.",
                    exception,
                    "The unit with control of the flow will be skipped.");

                // Skip of the unit
                return ProcessUnitAsync(unitNode.Next, data);
            }
        }
    }
}
