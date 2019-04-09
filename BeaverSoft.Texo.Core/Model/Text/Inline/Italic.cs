namespace BeaverSoft.Texo.Core.Model.Text
{
    public class Italic : IItalic
    {
        public Italic(IInline content)
        {
            Content = content;
        }

        public Italic(string text)
            : this(new PlainText(text))
        {
            // no operation
        }

        public IInline Content { get; }

        public override string ToString()
        {
            return $"*{Content}*";
        }
    }
}