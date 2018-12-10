using System;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public class DocumentBuilder
    {
        private readonly BlockCollection result;
        private BlockCollection context;

        public DocumentBuilder(BlockCollection newCollection)
        {
            result = newCollection ?? throw new ArgumentNullException(nameof(newCollection));
            context = result;
        }

        public static DocumentBuilder Create<T>()
            where T : BlockCollection, new()
        {
            return new DocumentBuilder(new T());
        }

        public static DocumentBuilder CreateDocument()
        {
            return new DocumentBuilder(new Document());
        }

        public static DocumentBuilder CreateSection()
        {
            return new DocumentBuilder(new Section());
        }

        public BlockCollection Build()
        {
            return result;
        }

        public T Build<T>()
            where T : BlockCollection
        {
            return (T)result;
        }

        public DocumentBuilder Header(string title)
        {
            context = context.AddChild(new Header(title));
            return this;
        }

        public ISpanBuilder StartHeader()
        {
            Span content = new Span();
            Header header = new Header(content);
            context = context.AddChild(header);
            return new SpanBuilder(content);
        }

    }
}
