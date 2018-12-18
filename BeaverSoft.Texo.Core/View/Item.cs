using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.View
{
    public class Item : IItem
    {
        private ImmutableList<IActionLink> actions;

        public Item(string text, TextFormatEnum format)
        {
            Text = text;
            Format = format;
            Actions = ImmutableList<IActionLink>.Empty;
        }

        public Item(string text)
            : this(text, TextFormatEnum.Undefined)
        {
            // no operation
        }

        protected Item()
            : this(string.Empty, TextFormatEnum.Undefined)
        {
            // no operation
        }

        public string Text { get; set; }

        public TextFormatEnum Format { get; set; }

        public IImmutableList<IActionLink> Actions { get; set; }

        public void AddActions(ImmutableList<IActionLink> actionsToAdd)
        {
            actions = actions.AddRange(actionsToAdd);
        }

        public void SetActions(ImmutableList<IActionLink> newActions)
        {
            actions = newActions;
        }

        public static implicit operator Item(string text)
        {
            return new Item(text);
        }

        public static explicit operator string(Item item)
        {
            return item.Text;
        }

        public static Item Plain(string text)
        {
            return new Item(text, TextFormatEnum.Plain);
        }

        public static Item Markdown(string text)
        {
            return new Item(text, TextFormatEnum.Markdown);
        }

        public static Item Html(string text)
        {
            return new Item(text, TextFormatEnum.Html);
        }

        public static Item Json(string text)
        {
            return new Item(text, TextFormatEnum.Json);
        }

        public static Item Xml(string text)
        {
            return new Item(text, TextFormatEnum.Xml);
        }
    }
}
