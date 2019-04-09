namespace BeaverSoft.Texo.Core.Model.Text
{
    public class Marked : IMarked
    {
        public Marked(IInline content)
        {
            Content = content;
        }

        public Marked(string text)
            : this(new PlainText(text))
        {
            // no operation
        }

        public IInline Content { get; }

        public override string ToString()
        {
            return $"=={Content}==";
        }
    }
}