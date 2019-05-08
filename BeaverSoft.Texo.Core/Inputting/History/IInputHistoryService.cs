using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Inputting.History
{
    public interface IInputHistoryService
    {
        int Count { get; }

        void Enqueue(Input input);

        IHistoryItem GetLastInput();

        IImmutableList<IHistoryItem> GetHistory();
    }
}