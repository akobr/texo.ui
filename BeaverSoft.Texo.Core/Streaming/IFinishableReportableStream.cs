namespace BeaverSoft.Texo.Core.Streaming
{
    public interface IFinishableReportableStream : IReportableStream
    { 
        void NotifyAboutCompletion();
    }
}