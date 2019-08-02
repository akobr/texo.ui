using System.Collections.Generic;
using System.Text.RegularExpressions;
using BeaverSoft.Texo.Core.Markdown.Builder;

namespace BeaverSoft.Texo.Core.Markdown
{
    public static class Markdown
    {
        private static readonly Regex escape = new Regex("([!\"#$%&'()*+,\\-\\.\\/:;<=>?@[\\\\\\]^_`{|}~])", RegexOptions.Compiled);

        public static string Encode(string text)
        {
            return escape.Replace(text, @"\$1");
        }

        public static string Header(string text)
        {
            return new MarkdownBuilder().Header(text).ToString();
        }

        public static string Header(string text, int level)
        {
            return new MarkdownBuilder().Header(text, level).ToString();
        }

        public static string Italic(string text)
        {
            return new MarkdownBuilder().Italic(text).ToString();
        }

        public static string Bold(string text)
        {
            return new MarkdownBuilder().Bold(text).ToString();
        }

        public static string CodeInline(string text)
        {
            return new MarkdownBuilder().CodeInline(text).ToString();
        }

        public static string CodeBlock(string language, string text)
        {
            return new MarkdownBuilder().CodeBlock(language, text).ToString();
        }

        public static string Blockquotes(string text)
        {
            return new MarkdownBuilder().Blockquotes(text).ToString();
        }

        public static string Link(string title, string path)
        {
            return new MarkdownBuilder().Link(title, path).ToString();
        }

        public static string List(params string[] items)
        {
            return List((IEnumerable<string>)items);
        }

        public static string List(IEnumerable<string> items)
        {
            var builder = new MarkdownBuilder();

            foreach (string item in items)
            {
                builder.Bullet(item);
            }

            return builder.ToString();
        }
    }
}
