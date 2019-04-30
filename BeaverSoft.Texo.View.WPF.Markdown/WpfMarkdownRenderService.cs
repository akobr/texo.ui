using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using BeaverSoft.Texo.Core.Services;
using BeaverSoft.Texo.Core.Streaming;
using BeaverSoft.Texo.Core.View;
using BeaverSoft.Texo.View.WPF.Rendering;

namespace BeaverSoft.Texo.View.WPF.Markdown
{
    public class WpfMarkdownRenderService : IWpfRenderService
    {
        private readonly IMarkdownService markdown;
        private readonly IWpfRenderService fallbackRenderService;

        public WpfMarkdownRenderService(IMarkdownService markdown)
        {
            this.markdown = markdown;
            fallbackRenderService = new WpfPlainTextRenderService();
        }

        public Section Render(IItem item)
        {
            if (item.Format != TextFormatEnum.Markdown
                && item.Format != TextFormatEnum.Model)
            {
                return fallbackRenderService.Render(item);
            }

            return BuildItem(item);
        }

        public Task<Section> StartStreamRenderAsync(IReportableStream stream, Action<Span> onAfterRender, Action onFinish)
        {
            return fallbackRenderService.StartStreamRenderAsync(stream, onAfterRender, onFinish);
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
    }
}
