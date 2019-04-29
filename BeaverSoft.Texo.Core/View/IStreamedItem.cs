using BeaverSoft.Texo.Core.Streaming.Text;

namespace BeaverSoft.Texo.Core.View
{
    public interface IStreamedItem : IItem
    {
        IReportableStream Stream { get; }
    }
}
