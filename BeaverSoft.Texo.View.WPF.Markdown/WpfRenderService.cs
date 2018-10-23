using System.Windows.Documents;
using BeaverSoft.Texo.Core.Services;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.View.WPF.Markdown
{
    public class WpfRenderService : IWpfRenderService
    {
        private readonly IMarkdownService markdown;

        public WpfRenderService(IMarkdownService markdown)
        {
            this.markdown = markdown;
        }

        public Section Render(IItem item)
        {
            return BuildItem(item);
        }

        private Section BuildItem(IItem item)
        {
            FlowDocument itemDocument = Markdig.Wpf.Markdown.ToFlowDocument(item.Text, markdown.Pipeline);
            Section itemSection = new Section();

            foreach (Block block in itemDocument.Blocks)
            {
                itemSection.Blocks.Add(block);
            }

            itemSection.Tag = item;
            return itemSection;
        }
    }
}
