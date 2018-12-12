using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public class List : IList
    {
        private readonly ImmutableList<IListItem> items;

        public List()
        {
            items = ImmutableList<IListItem>.Empty;
        }

        public List(IListItem content)
        {
            items = ImmutableList<IListItem>.Empty.Add(content);
        }

        public List(params IListItem[] content)
        {
            items = ImmutableList<IListItem>.Empty.AddRange(content);
        }

        private List(ImmutableList<IListItem> items)
        {
            this.items = items;
        }

        public IInline Content => null;

        public List AddItem(IListItem itemToAdd)
        {
            return new List(items.Add(itemToAdd));
        }

        public List AddItems(IEnumerable<IListItem> itemsToAdd)
        {
            return new List(items.AddRange(itemsToAdd));
        }

        public IEnumerator<IListItem> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            foreach (IListItem inline in items)
            {
                result.Append(inline);

                if (result.ToString(result.Length - System.Environment.NewLine.Length,
                        System.Environment.NewLine.Length) != System.Environment.NewLine)
                {
                    result.AppendLine();
                }
            }

            return result.ToString();
        }
    }
}