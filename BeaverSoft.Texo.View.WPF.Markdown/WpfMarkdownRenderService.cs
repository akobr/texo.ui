using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using BeaverSoft.Texo.Core.Services;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.View.WPF.Markdown
{
    public class WpfMarkdownRenderService : IWpfRenderService
    {
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
            FlowDocument itemDocument = Markdig.Wpf.Markdown.ToFlowDocument(item.Text, markdown.Pipeline);
            Section itemSection = new Section();
            List<Block> blocks = itemDocument.Blocks.ToList();

            foreach (Block block in blocks)
            {
                itemSection.Blocks.Add(block);
            }

            itemSection.Tag = item;
            return itemSection;
        }

        private static Section BuildPlainItem(IItem item)
        {
            Section itemSection = new Section();
            itemSection.Blocks.Add(new Paragraph(new Run(item.Text)));
            return itemSection;
        }
    }
}
