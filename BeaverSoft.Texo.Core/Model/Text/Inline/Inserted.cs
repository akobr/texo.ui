namespace BeaverSoft.Texo.Core.Model.Text
{
    public class Inserted : IInserted
    {
        public Inserted(IInline content)
        {
            Content = content;
        }

        public Inserted(string text)
            : this(new PlainText(text))
        {
            // no operation
        }

        public IInline Content { get; }

        public override string ToString()
        {
            return $"++{Content}++";
        }
    }
}