namespace BeaverSoft.Texo.Core.Model.Text
{
    public class Strong : IStrong
    {
        public Strong(IInline content)
        {
            Content = content;
        }

        public Strong(string text)
            : this(new PlainText(text))
        {
            // no operation
        }

        public IInline Content { get; }

        public override string ToString()
        {
            return $"**{Content}**";
        }
    }
}
