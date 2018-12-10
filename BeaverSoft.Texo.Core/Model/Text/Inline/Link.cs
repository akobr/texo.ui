using System.Text;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public class Link : ILink
    {
        public Link(IInline content, string address)
        {
            Content = content;
            Address = address;
        }

        public Link(string title, string address)
            : this(new PlainText(title), address)
        {
            // no operation
        }

        public string Address { get; }

        public IInline Content { get; }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[");
            result.Append(Content);
            result.Append("](");
            result.Append(Address);
            result.Append(")");
            return result.ToString();
        }
    }
}