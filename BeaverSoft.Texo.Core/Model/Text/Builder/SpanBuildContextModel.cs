using System;

namespace BeaverSoft.Texo.Core.Model.Text
{
    internal class SpanBuildContextModel
    {
        private readonly Func<Span, IInline> factoryMethod;
        private readonly Span parentSpan;
        private readonly int indexInParent;

        private ISpanBuildContext context;
        private Span buildedSpan;

        public SpanBuildContextModel(
            Func<Span, IInline> factoryMethod,
            Span parentSpan,
            int indexInParent,
            SpanBuilder builder)
        {
            this.factoryMethod = factoryMethod;
            this.parentSpan = parentSpan;
            this.indexInParent = indexInParent;
            Builder = builder;
        }

        public SpanBuilder Builder { get; }

        public void SetContext(ISpanBuildContext contextToSet)
        {
            context = contextToSet;
        }

        public void LockResultSpan(Span span)
        {
            buildedSpan = span;
        }

        public Span GetBuildedSpan()
        {
            return buildedSpan ?? Builder.Span;
        }

        public Span BuildModifieredParent()
        {
            return parentSpan.InsertAt(indexInParent, factoryMethod(context.Span));
        }
    }
}