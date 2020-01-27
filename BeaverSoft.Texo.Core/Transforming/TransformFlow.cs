using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BeaverSoft.Texo.Core.Transforming
{
    public class TransformFlow<TData>
    {
        private readonly LinkedList<TransformBlock<TData, TData>> flow;

        public TransformFlow()
        {
            flow = new LinkedList<TransformBlock<TData, TData>>();
        }

        public Task Completion => GetLastBlockOrThrow().Completion;

        public void Send(TData item)
        {
            GetFirstBlockOrThrow().Post(item);
        }

        public Task SendAsync(TData item)
        {
            return GetFirstBlockOrThrow().SendAsync(item);
        }

        public void Complete()
        {
            GetFirstBlockOrThrow().Complete();
        }

        public void AddTransformation(Func<TData, TData> transformation)
        {
            var lastNode = flow.Last;
            var newBlock = new TransformBlock<TData, TData>(transformation);

            flow.AddLast(newBlock);

            if (lastNode == null)
            {
                return;
            }

            lastNode.Value.LinkTo(newBlock, new DataflowLinkOptions { PropagateCompletion = true });
        }

        private TransformBlock<TData, TData> GetFirstBlockOrThrow()
        {
            return flow.First?.Value ?? throw new InvalidOperationException("The trasformation flow is empty.");
        }

        private TransformBlock<TData, TData> GetLastBlockOrThrow()
        {
            return flow.Last?.Value ?? throw new InvalidOperationException("The trasformation flow is empty.");
        }
    }
}
