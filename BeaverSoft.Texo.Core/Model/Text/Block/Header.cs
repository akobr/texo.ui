using System;
using System.Text;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public class Header : IHeader
    {
        public Header(string title)
            : this(new PlainText(title))
        {
            // no operation
        }

        public Header(ushort level, string title)
            : this(new PlainText(title))
        {
            Level = Math.Max(level, (ushort)1);
        }

        public Header(IInline content)
        {
            Content = content;
        }

        public Header(ushort level, IInline content)
            : this(content)
        {
            Level = Math.Max(level, (ushort)1);
        }

        public IInline Content { get; }

        public ushort? Level { get; }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            if (Level.HasValue)
            {
                result.Append('#', Level.Value);
                result.Append(' ');
            }
            else
            {
                result.Append("X# ");
            }

            result.Append(Content);
            return result.ToString();
        }
    }
}