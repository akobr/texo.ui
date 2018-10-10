namespace BeaverSoft.Texo.Core.Model.View
{
    public class Item : IItem
    {
        public Item(string text)
        {
            Text = text;
        }

        protected Item()
            : this(string.Empty)
        {
            // no operation
        }

        public string Text { get; set; }

        public static implicit operator Item(string text)
        {
            return new Item(text);
        }

        public static explicit operator string(Item item)
        {
            return item.Text;
        }
    }
}
