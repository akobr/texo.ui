using System;
using System.Drawing;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public class SpanBuildContext : ISpanBuildContext
    {
        private readonly Span span;
        private readonly Guid identifier;
        private readonly SpanBuilder builder;

        private bool disposed;

        public SpanBuildContext(Span span, Guid identifier, SpanBuilder builder)
        {
            this.span = span;
            this.identifier = identifier;
            this.builder = builder;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            builder.EndContext(identifier);
        }

        public Span Build()
        {
            return span;
        }

        public ISpanBuilder Write(string text)
        {
            builder.Write(text);
            return this;
        }

        public ISpanBuilder WriteLine(string text)
        {
            builder.WriteLine(text);
            return this;
        }

        public ISpanBuilder WriteLine()
        {
            builder.WriteLine();
            return this;
        }

        public ISpanBuilder Strong(string text)
        {
            builder.Strong(text);
            return this;
        }

        public ISpanBuilder Italic(string text)
        {
            builder.Italic(text);
            return this;
        }

        public ISpanBuilder Strikethrough(string text)
        {
            builder.Strikethrough(text);
            return this;
        }

        public ISpanBuilder Marked(string text)
        {
            builder.Marked(text);
            return this;
        }

        public ISpanBuilder Inserted(string text)
        {
            builder.Inserted(text);
            return this;
        }

        public ISpanBuilder Code(string text)
        {
            builder.Code(text);
            return this;
        }

        public ISpanBuilder Highlight(string text, Color highlightColor)
        {
            builder.Highlight(text, highlightColor);
            return this;
        }

        public ISpanBuildContext StartStrongContext()
        {
            return builder.StartStrongContext();
        }

        public ISpanBuildContext StartItalicContext()
        {
            return builder.StartItalicContext();
        }

        public ISpanBuildContext StartMarkedContext()
        {
            return builder.StartMarkedContext();
        }

        public ISpanBuildContext StartInsertedContext()
        {
            return builder.StartInsertedContext();
        }

        public ISpanBuildContext StartHighlightContext(Color highlightColor)
        {
            return builder.StartHighlightContext(highlightColor);
        }

        public ISpanBuilder EndContext()
        {
            builder.EndContext();
            return this;
        }
    }
}
