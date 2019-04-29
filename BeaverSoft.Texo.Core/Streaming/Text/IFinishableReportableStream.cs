namespace BeaverSoft.Texo.Core.Streaming.Text
{
    public interface IFinishableReportableStream : IReportableStream
    { 
        void NotifyAboutCompletion();
    }
}