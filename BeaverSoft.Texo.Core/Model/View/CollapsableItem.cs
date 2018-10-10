namespace BeaverSoft.Texo.Core.Model.View
{
    public class CollapsableItem : Item, ICollapsableItem
    {
        public CollapsableItem(string text, string content)
            : base(text)
        {
            Content = content;
        }

        public CollapsableItem()
            : this(string.Empty, string.Empty)
        {
            // no operation
        }

        public string Content { get; set; }

        public bool IsExpanded { get; set; }
    }
}
