using System.Windows.Controls;
using System.Windows.Documents;
using BeaverSoft.Texo.Core.Model.View;
using BeaverSoft.Texo.Core.Services;

namespace BeaverSoft.Texo.View.WPF.Markdown
{
    public class WpfRenderService : IWpfRenderService
    {
        private readonly IMarkdownService markdown;
        private readonly FlowDocumentScrollViewer viewer;

        public WpfRenderService(IMarkdownService markdown)
        {
            this.markdown = markdown;
        }

        public void Render(IItem item)
        {
            viewer.Document.Blocks.Add(BuildItem(item));
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
