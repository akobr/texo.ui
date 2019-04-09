using System.Text;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public class OrderedListItem : ListItem
    {
        public OrderedListItem(IInline content)
            : base(1, content)
        {
            // no operation
        }

        public OrderedListItem(ushort number, IInline content)
            : base(1, content)
        {
            Number = number;
        }

        public OrderedListItem(ushort number, ushort level, IInline content)
            : base(level, content)
        {
            Number = number;
        }

        public ushort? Number { get; }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append(' ', Level);

            if (Number.HasValue)
            {
                result.Append(Number);
            }
            else
            {
                result.Append('X');
            }

            result.Append(". ");
            result.Append(Content);
            return result.ToString();
        }
    }
}