using System;
using System.Collections.Generic;
using System.Drawing;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public class SpanBuilder : ISpanBuilder
    {
        private readonly object contextLock = new object();
        private readonly Stack<SpanBuildContextModel> stack;

        private Span current;
        private Span rootSpanHolder;

        public SpanBuilder()
        {
            current = new Span();
            stack = new Stack<SpanBuildContextModel>();
        }

        public static ISpanBuilder Create()
        {
            return new SpanBuilder();
        }

        public Span Span => rootSpanHolder ?? current;

        public ISpanBuilder Write(string text)
        {
            current = current.AddChild(new PlainText(text));
            return this;
        }

        public ISpanBuilder WriteLine(string text)
        {
            current = current.AddChild(new PlainText(text));
            return WriteLine();
        }

        public ISpanBuilder WriteLine()
        {
            current = current.AddChild(new PlainText(System.Environment.NewLine));
            return this;
        }

        public ISpanBuilder Strong(string text)
        {
            current = current.AddChild(new Strong(text));
            return this;
        }

        public ISpanBuilder Italic(string text)
        {
            current = current.AddChild(new Italic(text));
            return this;
        }

        public ISpanBuilder Strikethrough(string text)
        {
            current = current.AddChild(new Strikethrough(text));
            return this;
        }

        public ISpanBuilder Marked(string text)
        {
            current = current.AddChild(new Marked(text));
            return this;
        }

        public ISpanBuilder Inserted(string text)
        {
            current = current.AddChild(new Inserted(text));
            return this;
        }

        public ISpanBuilder Code(string text)
        {
            current = current.AddChild(new CodeInline(text));
            return this;
        }

        public ISpanBuilder Highlight(string text, Color highlightColor)
        {
            current = current.AddChild(new Highlight(text, highlightColor));
            return this;
        }

        public ISpanBuilder Link(string title, string address)
        {
            current = current.AddChild(new Link(title, address));
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

            SpanBuildContextModel model = stack.Pop();
            model.LockResultSpan(current);
            current = model.BuildModifieredParent();

            if (stack.Count < 1)
            {
                rootSpanHolder = null;
            }

            return this;
        }

        internal void EndContext(SpanBuildContextModel model)
        {
            lock (contextLock)
            {
                if (stack.Peek() != model)
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
                if (rootSpanHolder == null)
                {
                    rootSpanHolder = current;
                }

                SpanBuildContextModel model = new SpanBuildContextModel(factoryMethod, current, current.Children.Count, this);
                SpanBuildContext context = new SpanBuildContext(model);

                stack.Push(model);
                current = new Span();
                return context;
            }
        }
    }
}