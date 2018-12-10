using System.Drawing;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public class Highlight : IHighlight
    {
        public Highlight(IInline content, Color highlightColor)
        {
            Content = content;
            Color = highlightColor;
        }

        public Highlight(string text, Color highlightColor)
            : this(new PlainText(text), highlightColor)
        {
            // no operation
        }

        public IInline Content { get; }

        public Color Color { get; }

        public override string ToString()
        {
            return $"`{Content}`{{{Color}}}";
        }
    }
}