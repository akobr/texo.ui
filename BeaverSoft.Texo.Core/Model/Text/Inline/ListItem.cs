using System;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public class ListItem : IListItem
    {
        public ListItem(IInline content)
            : this(1, content)
        {
            // no operation
        }

        public ListItem(string text)
            : this(new PlainText(text))
        {
            // no operation
        }

        public ListItem(ushort level, IInline content)
        {
            Content = content;
            Level = Math.Max(level, (ushort)1);
        }

        public ListItem(ushort level, string text)
            : this(level, new PlainText(text))
        {
            // no operation
        }

        public IInline Content { get; }

        public ushort Level { get; }

        public override string ToString()
        {
            return $"{new string(' ', Math.Max((Level - 1) * 2, 1))}* {Content}";
        }
    }
}