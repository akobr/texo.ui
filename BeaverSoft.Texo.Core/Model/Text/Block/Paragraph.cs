﻿namespace BeaverSoft.Texo.Core.Model.Text
{
    public class Paragraph : IParagraph
    {
        public Paragraph(IInline content)
        {
            Content = content;
        }

        public Paragraph(ISpanBuilder contentBuilder)
            : this(contentBuilder.Span)
        {
            // no operation
        }

        public Paragraph(string text)
            : this(new PlainText(text))
        {
            // no operation
        }

        public IInline Content { get; }

        public string ToString(int level)
        {
            return ToString();
        }

        public override string ToString()
        {
            return Content.ToString();
        }
    }
}