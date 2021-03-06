using BeaverSoft.Texo.Core.Streaming;
using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.View
{
    public class StreamedItem : IStreamedItem
    {
        public StreamedItem(IReportableStream stream)
        {
            Stream = stream;
        }

        public IReportableStream Stream { get; }

        public string Text => Stream.ReadFromBeginningToEnd();

        public TextFormatEnum Format => TextFormatEnum.Plain;

        public IImmutableList<ILink> Actions => ImmutableList<ILink>.Empty;

        public static implicit operator StreamedItem(ReportableStream stream)
        {
            return new StreamedItem(stream);
        }
    }
}
