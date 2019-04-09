using System;
using System.Text;
using BeaverSoft.Texo.Core.Services;
using BeaverSoft.Texo.Core.View;
using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

using SysConsole = System.Console;

namespace BeaverSoft.Texo.View.Console.Markdown
{
    public class ConsoleMarkdownRenderService : IConsoleRenderService
    {
        private readonly IMarkdownService markdown;

        public ConsoleMarkdownRenderService(IMarkdownService markdown)
        {
            this.markdown = markdown;
        }

        public void Write(IItem item)
        {
            if (item.Format == TextFormatEnum.Plain)
            {
                SysConsole.Write(item.Text);
            }
            else
            {
                MarkdownDocument doc = markdown.Parse(item.Text);
                WriteBlockContainer(doc);
            }

            SysConsole.WriteLine();
        }

        private static void WriteBlockContainer(ContainerBlock container)
        {
            bool needNewLine = false;

            foreach (Block block in container)
            {
                if (needNewLine)
                {
                    SysConsole.WriteLine();
                }

                switch (block)
                {
                    case HeadingBlock heading:
                        MarkdownConsole.WriteHeader(GetTextFromInlineContainer(heading.Inline), heading.Level);
                        needNewLine = true;
                        break;

                    case CodeBlock code:
                        WriteCode(code);
                        break;

                    case QuoteBlock quote:
                        MarkdownConsole.WriteQuote(GetTextFromBlockContainer(quote));
                        break;

                    case ThematicBreakBlock horizontalLine:
                        SysConsole.WriteLine();
                        MarkdownConsole.WriteHorizontalBreak();
                        SysConsole.WriteLine();
                        break;

                    case ParagraphBlock paragraph:
                        WriteLeafBlock(paragraph);
                        break;

                    case ListBlock list:
                        WriteList(list);
                        break;

                    case HtmlBlock html:
                        WriteHtml(html);
                        break;
                }
            }
        }

        private static void WriteCode(CodeBlock code)
        {
            foreach (StringLine line in code.Lines)
            {
                MarkdownConsole.WriteCode(GetTextFromSlice(line.Slice));
            }
            SysConsole.WriteLine();
        }

        private static void WriteList(ListBlock list, int intentLevel = 1)
        {
            int.TryParse(list.OrderedStart, out int index);
            intentLevel = Math.Max(intentLevel, 1);
            bool needNewLine = false;

            foreach (Block item in list)
            {
                if (needNewLine)
                {
                    SysConsole.WriteLine();
                }

                switch (item)
                {
                    case ListItemBlock listItem:
                        MarkdownConsole.WriteListItemBullet(
                            list.IsOrdered
                                ? $"{index++}{list.OrderedDelimiter}"
                                : GetListItemBullet(intentLevel),
                            intentLevel);
                        WriteBlockContainer(listItem);
                        needNewLine = true;
                        break;

                    case ListBlock childList:
                        WriteList(childList, intentLevel + 1);
                        needNewLine = true;
                        break;
                }
            }
        }

        public static void WriteLeafBlock(LeafBlock paragraph)
        {
            foreach (Inline inline in paragraph.Inline)
            {
                WriteInline(inline);
            }
        }

        private static string GetListItemBullet(int intentLevel)
        {
            return "*";
        }

        public static void WriteHtml(HtmlBlock html)
        {
            if (html.Type != HtmlBlockType.InterruptingBlock
                && html.Type != HtmlBlockType.NonInterruptingBlock)
            {
                return;
            }

            MarkdownConsole.WriteHtml(GetTextFromInlineContainer(html.Inline));
        }

        private static void WriteInline(IInline inline)
        {
            switch (inline)
            {
                case LiteralInline literal:
                    SysConsole.Write(GetTextFromLiteral(literal));
                    return;

                case LinkInline link:
                    MarkdownConsole.WriteLink(GetUrlFromLink(link), link.Title);
                    return;

                case AutolinkInline autoLink:
                    MarkdownConsole.WriteLink(autoLink.Url);
                    return;

                case CodeInline code:
                    MarkdownConsole.WriteCode(code.Content);
                    return;

                case EmphasisInline emphasis:
                    string text = GetTextFromInlineContainer(emphasis);
                    if (emphasis.IsDouble)
                    {
                        MarkdownConsole.WriteBold(text);
                    }
                    else
                    {
                        MarkdownConsole.WriteItalic(text);
                    }
                    return;

                case HtmlInline html:
                    MarkdownConsole.WriteHtml(html.Tag);
                    return;

                case HtmlEntityInline htmlEntity:
                    MarkdownConsole.WriteHtml(htmlEntity.Transcoded.Text);
                    return;

                case LineBreakInline lineBreak:
                    SysConsole.WriteLine();
                    return;
            }
        }

        private static string GetText(IInline inline)
        {
            switch (inline)
            {
                case LiteralInline literal:
                    return GetTextFromLiteral(literal);

                case LinkInline link:
                    return GetTextFromLink(link);

                case AutolinkInline autoLink:
                    return autoLink.Url;

                case CodeInline code:
                    return code.Content;

                case EmphasisInline emphasis:
                    return GetTextFromInlineContainer(emphasis);

                case HtmlInline html:
                    return html.Tag;

                case HtmlEntityInline htmlEntity:
                    return htmlEntity.Transcoded.Text;

                case LineBreakInline lineBreak:
                    return Environment.NewLine;

                default:
                    return string.Empty;
            }
        }

        private static string GetTextFromSlice(StringSlice slice)
        {
            if (slice.Text == null)
            {
                return string.Empty;
            }

            return slice.Text.Substring(slice.Start, slice.Length);
        }

        private static string GetTextFromLiteral(LiteralInline literal)
        {
            return GetTextFromSlice(literal.Content);
        }

        private static string GetTextFromLink(LinkInline link)
        {
            string title = link.Title;
            string url = GetUrlFromLink(link);

            if (string.IsNullOrWhiteSpace(title))
            {
                return url;
            }

            return $"{title} / {url}";
        }

        private static string GetTextFromBlockContainer(ContainerBlock container)
        {
            StringBuilder builder = new StringBuilder();
            GetTextFromBlockContainer(container, builder);
            return builder.ToString();
        }

        private static void GetTextFromBlockContainer(ContainerBlock container, StringBuilder builder)
        {
            foreach (Block block in container)
            {
                switch (block)
                {
                    case LeafBlock leaf:
                        GetTextFromInlineContainer(leaf.Inline, builder);
                        break;

                    case ContainerBlock childContainer:
                        GetTextFromBlockContainer(childContainer, builder);
                        break;
                }
            }
        }

        private static string GetTextFromInlineContainer(ContainerInline container)
        {
            StringBuilder builder = new StringBuilder();
            GetTextFromInlineContainer(container, builder);
            return builder.ToString();
        }

        private static void GetTextFromInlineContainer(ContainerInline container, StringBuilder builder)
        {
            foreach (Inline child in container)
            {
                builder.Append(GetText(child));
            }
        }

        private static string GetUrlFromLink(LinkInline link)
        {
            if (link == null)
            {
                return string.Empty;
            }

            if (link.IsShortcut && link.Reference != null)
            {
                return link.Reference.Url;
            }

            if (link.GetDynamicUrl != null)
            {
                return link.GetDynamicUrl.Invoke();
            }

            return link.Url;
        }
    }
}