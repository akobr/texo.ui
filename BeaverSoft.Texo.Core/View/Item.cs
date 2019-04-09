using BeaverSoft.Texo.Core.Actions;
using System.Collections.Immutable;
using System.Text;

namespace BeaverSoft.Texo.Core.View
{
    public class Item : IItem
    {
        public Item(string text, TextFormatEnum format)
        {
            Text = text;
            Format = format;
            Actions = ImmutableList<ILink>.Empty;
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

        public IImmutableList<ILink> Actions { get; set; }

        public void AddActions(ImmutableList<ILink> links)
        {
            Actions = Actions.AddRange(links);
        }

        public void AddAction(ILink link)
        {
            Actions = Actions.Add(link);
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

        public static Item Intellisence(string item, string type, string description)
        {
            return Intellisence(item, item, type, description);
        }

        public static Item Intellisence(string item, string inputUpdate, string type, string description)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"**{item}**");

            if (!string.IsNullOrEmpty(type))
            {
                builder.Append($" *({type})*");
            }

            if (!string.IsNullOrEmpty(type))
            {
                builder.Append($": {description}");
            }

            Item viewItem = Markdown(builder.ToString());
            viewItem.AddAction(new Link("Use", ActionBuilder.InputUpdateUri(inputUpdate)));
            return viewItem;
        }
    }
}
