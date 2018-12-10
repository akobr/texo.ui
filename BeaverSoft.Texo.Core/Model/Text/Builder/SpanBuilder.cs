using System;
using System.Collections.Generic;
using System.Drawing;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public class SpanBuilder : ISpanBuilder
    {
        private readonly object contextLock = new object();

        private readonly Span result;
        private readonly Stack<Span> stack;
        private readonly Stack<Guid> stackGuid;

        private Span context;

        public SpanBuilder(Span target)
        {
            result = target ?? throw new ArgumentNullException(nameof(target));
            stack = new Stack<Span>();
            stackGuid = new Stack<Guid>();
            context = result;
        }

        public SpanBuilder()
            : this(new Span())
        {
            // no operation
        }

        public static ISpanBuilder Create()
        {
            return new SpanBuilder();
        }

        public Span Build()
        {
            return result;
        }

        public ISpanBuilder Write(string text)
        {
            context = context.AddChild(new PlainText(text));
            return this;
        }

        public ISpanBuilder WriteLine(string text)
        {
            context = context.AddChild(new PlainText(text));
            return WriteLine();
        }

        public ISpanBuilder WriteLine()
        {
            context = context.AddChild(new PlainText(System.Environment.NewLine));
            return this;
        }

        public ISpanBuilder Strong(string text)
        {
            context = context.AddChild(new Strong(text));
            return this;
        }

        public ISpanBuilder Italic(string text)
        {
            context = context.AddChild(new Italic(text));
            return this;
        }

        public ISpanBuilder Strikethrough(string text)
        {
            context = context.AddChild(new Strikethrough(text));
            return this;
        }

        public ISpanBuilder Marked(string text)
        {
            context = context.AddChild(new Marked(text));
            return this;
        }

        public ISpanBuilder Inserted(string text)
        {
            context = context.AddChild(new Inserted(text));
            return this;
        }

        public ISpanBuilder Code(string text)
        {
            context = context.AddChild(new CodeInline(text));
            return this;
        }

        public ISpanBuilder Highlight(string text, Color highlightColor)
        {
            context = context.AddChild(new Highlight(text, highlightColor));
            return this;
        }

        public ISpanBuildContext StartStrongContext()
        {
            return StartContext(span => new Strong(span));
        }

        public ISpanBuildContext StartItalicContext()
        {
            return StartContext(span => new Italic(span));
        }

        public ISpanBuildContext StartMarkedContext()
        {
            return StartContext(span => new Marked(span));
        }

        public ISpanBuildContext StartInsertedContext()
        {
            return StartContext(span => new Inserted(span));
        }

        public ISpanBuildContext StartHighlightContext(Color highlightColor)
        {
            return StartContext(span => new Highlight(span, highlightColor));
        }

        public ISpanBuilder EndContext()
        {
            if (stack.Count < 1)
            {
                return this;
            }

            stackGuid.Pop();
            context = stack.Pop();
            return this;
        }

        internal void EndContext(Guid guid)
        {
            lock (contextLock)
            {
                Guid currentContextGuid = stackGuid.Peek();

                if (guid != currentContextGuid)
                {
                    return;
                }

                EndContext();
            }
        }

        private SpanBuildContext StartContext(Func<Span, IInline> factoryMethod)
        {
            lock (contextLock)
            {
                Guid newGuid = Guid.NewGuid();
                Span newContext = new Span();

                stackGuid.Push(newGuid);
                stack.Push(context.AddChild(factoryMethod(newContext)));

                context = newContext;
                return new SpanBuildContext(context, newGuid, this);
            }
        }
    }
}