namespace BeaverSoft.Texo.Core.Model.Text
{
    public class PlainText : IPlainText
    {
        public PlainText(string text)
        {
            Text = text;
        }

        public string Text { get; }

        public IInline Content => null;

        public override string ToString()
        {
            return Text;
        }

        public static implicit operator PlainText(string text)
        {
            return new PlainText(text);
        }

        public static explicit operator string(PlainText text)
        {
            return text?.Text ?? string.Empty;
        }
    }
}
