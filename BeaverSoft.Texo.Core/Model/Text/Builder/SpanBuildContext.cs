using System.Drawing;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public class SpanBuildContext : ISpanBuildContext
    {
        private readonly SpanBuildContextModel model;
        private readonly SpanBuilder builder;
        private bool disposed;

        internal SpanBuildContext(SpanBuildContextModel model)
        {
            this.model = model;
            model.SetContext(this);
            builder = model.Builder;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            builder.EndContext(model);
        }

        public Span Span => model.GetBuildedSpan();

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
