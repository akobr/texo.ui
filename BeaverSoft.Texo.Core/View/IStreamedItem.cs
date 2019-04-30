using BeaverSoft.Texo.Core.Streaming;

namespace BeaverSoft.Texo.Core.View
{
    public interface IStreamedItem : IItem
    {
        IReportableStream Stream { get; }
    }
}
