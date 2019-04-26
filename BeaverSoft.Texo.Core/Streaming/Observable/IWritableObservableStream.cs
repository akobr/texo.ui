using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Streaming.Observable
{
    public interface IWritableObservableStream<TItem> : IObservableStream<TItem>
    {
        void Write(IEnumerable<TItem> items);

        Task WriteAsync(IEnumerable<TItem> items);
    }
}
