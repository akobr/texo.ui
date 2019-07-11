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

        public static Item Empty { get; } = AsPlain(string.Empty);

        // TODO: [P3] this is mutable (:
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

        public static Plain AsPlain(string text)
        {
            return new Plain(text);
        }

        public static Markdown AsMarkdown(string text)
        {
            return new Markdown(text);
        }

        public static Html AsHtml(string text)
        {
            return new Html(text);
        }

        public static Json AsJson(string text)
        {
            return new Json(text);
        }

        public static Yaml AsYaml(string text)
        {
            return new Yaml(text);
        }

        public static Item AsXml(string text)
        {
            return new Xml(text);
        }

        public static Item AsIntellisense(string item, string type, string description)
        {
            return AsIntellisense(item, item, type, description);
        }

        public static Item AsIntellisense(string item, string inputUpdate, string type, string description)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"**{item}**");

            if (!string.IsNullOrEmpty(type))
            {
                builder.Append($" *({type})*");
            }

            if (!string.IsNullOrEmpty(description))
            {
                builder.Append($": {description}");
            }

            Item viewItem = AsMarkdown(builder.ToString());
            viewItem.AddAction(new Link("Use", ActionBuilder.InputUpdateUri(inputUpdate)));
            return viewItem;
        }

        public class Plain : Item
        {
            public Plain(string text)
                : base(text, TextFormatEnum.Plain)
            {
                // no operation
            }

            public static implicit operator Plain(string text)
            {
                return new Plain(text);
            }
        }

        public class Markdown : Item
        {
            public Markdown(string text)
                : base(text, TextFormatEnum.Markdown)
            {
                // no operation
            }

            public static implicit operator Markdown(string text)
            {
                return new Markdown(text);
            }
        }

        public class Html : Item
        {
            public Html(string text)
                : base(text, TextFormatEnum.Html)
            {
                // no operation
            }

            public static implicit operator Html(string text)
            {
                return new Html(text);
            }
        }

        public class Json : Item
        {
            public Json(string text)
                : base(text, TextFormatEnum.Json)
            {
                // no operation
            }

            public static implicit operator Json(string text)
            {
                return new Json(text);
            }
        }

        public class Yaml : Item
        {
            public Yaml(string text)
                : base(text, TextFormatEnum.Yaml)
            {
                // no operation
            }

            public static implicit operator Yaml(string text)
            {
                return new Yaml(text);
            }
        }

        public class Xml : Item
        {
            public Xml(string text)
                : base(text, TextFormatEnum.Xml)
            {
                // no operation
            }

            public static implicit operator Xml(string text)
            {
                return new Xml(text);
            }
        }
    }
}
