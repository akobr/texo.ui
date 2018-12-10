namespace BeaverSoft.Texo.Core.Model.Text
{
    public class CodeInline : ICodeInline
    {
        public CodeInline(IInline content)
        {
            Content = content;
        }

        public CodeInline(string text)
            : this(new PlainText(text))
        {
            // no operation
        }

        public IInline Content { get; }

        public override string ToString()
        {
            return $"`{Content}`";
        }
    }
}