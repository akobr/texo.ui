using System;

namespace BeaverSoft.Texo.Core.Streaming.Observable
{
    public interface IObservableStream<TItem> : IDisposable
    {
        bool IsClosed { get; }

        void RegisterObserver(IStreamObserver observer);

        void Open();

        void Close();
    }
}
