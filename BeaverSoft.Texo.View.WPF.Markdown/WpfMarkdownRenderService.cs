using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Media;
using BeaverSoft.Texo.Core.Services;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.View.WPF.Markdown
{
    public class WpfMarkdownRenderService : IWpfRenderService
    {
        private readonly Regex coloringRegex = new Regex(@"(\u001b\[(?<color>\d*)m)(?<text>[^\u001b]*)(\u001b\[m)", RegexOptions.Compiled);
        private readonly IMarkdownService markdown;

        public WpfMarkdownRenderService(IMarkdownService markdown)
        {
            this.markdown = markdown;
        }

        public Section Render(IItem item)
        {
            if (item.Format != TextFormatEnum.Markdown
                && item.Format != TextFormatEnum.Model)
            {
                return BuildPlainItem(item);
            }

            return BuildItem(item);
        }

        private Section BuildItem(IItem item)
        {
            Section itemSection = BuildMarkdown(item.Text);
            itemSection.Tag = item;
            return itemSection;
        }

        private Section BuildMarkdown(string markdownText)
        {
            FlowDocument itemDocument = Markdig.Wpf.Markdown.ToFlowDocument(markdownText, markdown.Pipeline);
            Section itemSection = new Section();
            List<Block> blocks = itemDocument.Blocks.ToList();

            foreach (Block block in blocks)
            {
                itemSection.Blocks.Add(block);
            }

            return itemSection;
        }

        //private string ColorReplacement(Match match)
        //{
        //    string colorNumber = match.Groups["color"].Value;
        //    string text = match.Groups["text"].Value;

        //    switch (colorNumber)
        //    {
        //        case "31": // red
        //            return $"**{text}**";

        //        case "32": // green
        //            return $"*{text}*";

        //        case "33": // yellow
        //            return $"++{text}++";

        //        case "34": // blue
        //            return $"=={text}==";

        //        case "35": // magenta
        //            return $"~~{text}~~";

        //        case "36": // cyan
        //            return $"~~{text}~~";

        //        default: // white
        //            return text;
        //    }
        //}

        private Brush ColorReplacement(Match match)
        {
            string colorNumber = match.Groups["color"].Value;

            switch (colorNumber)
            {
                case "31": // red
                    return Brushes.Red;

                case "32": // green
                    return Brushes.Green;

                case "33": // yellow
                    return Brushes.Yellow;

                case "34": // blue
                    return Brushes.Blue;

                case "35": // magenta
                    return Brushes.Magenta;

                case "36": // cyan
                    return Brushes.Cyan;

                default: // white
                    return Brushes.Gray;
            }
        }

        private Section BuildPlainItem(IItem item)
        {
            // (\n[^\n\x{001b}]*)(\x{001b}\[m)
            //string text = coloringRegex.Replace(item.Text, ColorReplacement);
            Section itemSection = new Section();
            Paragraph paragraph = new Paragraph();
            Inline nextSibling = new Run(string.Empty);
            paragraph.Inlines.Add(nextSibling);

            var matches = coloringRegex.Matches(item.Text);
            int index = item.Text.Length;

            for(int i = matches.Count - 1; i >= 0; i--)
            {
                Match match = matches[i];
                int nextIndex = match.Index + match.Length;

                if (index > nextIndex)
                {
                    Inline newInline = new Run(item.Text.Substring(nextIndex, index - nextIndex));
                    paragraph.Inlines.InsertBefore(nextSibling, newInline);
                    nextSibling = newInline;
                    index = nextIndex;
                }

                Inline replacementInline = new Run(match.Groups["text"].Value);
                replacementInline.Foreground = ColorReplacement(match);
                paragraph.Inlines.InsertBefore(nextSibling, replacementInline);
                nextSibling = replacementInline;
                index = match.Index;
            }

            if (index > 0)
            {
                Inline newInline = new Run(item.Text.Substring(0, index));
                paragraph.Inlines.InsertBefore(nextSibling, newInline);
            }

            itemSection.Blocks.Add(paragraph);
            return itemSection;
        }
    }
}
