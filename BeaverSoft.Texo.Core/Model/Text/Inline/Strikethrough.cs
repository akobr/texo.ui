namespace BeaverSoft.Texo.Core.Model.Text
{
    public class Strikethrough : IStrikethrough
    {
        public Strikethrough(IInline content)
        {
            Content = content;
        }

        public Strikethrough(string text)
            : this(new PlainText(text))
        {
            // no operation
        }

        public IInline Content { get; }

        public override string ToString()
        {
            return $"~~{Content}~~";
        }
    }
}