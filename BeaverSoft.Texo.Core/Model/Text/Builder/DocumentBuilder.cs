using System;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public class DocumentBuilder
    {
        private BlockCollection current;

        public DocumentBuilder(BlockCollection newCollection)
        {
            current = newCollection ?? throw new ArgumentNullException(nameof(newCollection));
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

        public BlockCollection Result => current;

        public T GetResult<T>()
            where T : BlockCollection
        {
            return (T)current;
        }

        public DocumentBuilder Header(string title)
        {
            current = current.AddChild(new Header(title));
            return this;
        }

        //public ISpanBuilder StartHeader()
        //{
        //    ISpanBuilder builder = new SpanBuilder();
        //    Header header = new Header(builder.Span);
        //    current = current.AddChild(header);
        //    return builder;
        //}
    }
}
