using System;
using System.Threading.Tasks;
using System.Windows.Documents;
using BeaverSoft.Texo.Core.Streaming;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.View.WPF.Rendering
{
    public class WpfPlainTextRenderService : IWpfRenderService
    {
        private readonly IWpfRenderService streamRenderService;

        public WpfPlainTextRenderService()
        {
            streamRenderService = new WpfConsoleTextRenderService();
        }

        public Section Render(IItem item)
        {
            // Plain text
            // Section itemSection = new Section();
            // itemSection.Blocks.Add(new Paragraph(new Run(item.Text)));
            // return itemSection;

            // Console like
            return streamRenderService.Render(item);
        }

        public Task<Section> StartStreamRenderAsync(IReportableStream stream, Action<Span> onAfterRender, Action onFinish)
        {
            return streamRenderService.StartStreamRenderAsync(stream, onAfterRender, onFinish);
        }
    }
}
