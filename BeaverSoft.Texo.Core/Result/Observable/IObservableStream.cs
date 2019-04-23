using System;

namespace BeaverSoft.Texo.Core.Result.Observable
{
    public interface IObservableStream<TItem> : IDisposable
    {
        bool IsOpen { get; }

        void RegisterObserver(IStreamObserver observer);

        void Open();

        void Close();
    }
}
