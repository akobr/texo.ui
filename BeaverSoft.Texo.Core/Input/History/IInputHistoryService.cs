using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Input.History
{
    public interface IInputHistoryService
    {
        int Count { get; }

        void Enqueue(Input input);

        IHistoryItem GetLastInput();

        IImmutableList<IHistoryItem> GetHistory();
    }
}