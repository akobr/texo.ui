using System;
using System.Threading.Tasks;
using System.Windows.Documents;
using BeaverSoft.Texo.Core.Streaming;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.View.WPF
{
    public interface IWpfRenderService
    {
        Section Render(IItem item);

        Task<Section> StartStreamRenderAsync(IReportableStream stream, Action<Span> onAfterRender, Action onFinish);
    }
}