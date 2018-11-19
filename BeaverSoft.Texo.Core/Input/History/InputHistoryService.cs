using System.Collections.Generic;
using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Input.History
{
    public class InputHistoryService : IInputHistoryService
    {
        private const int HISTORY_CAPACITY = 20;
        private readonly LinkedList<HistoryItem> history;

        public InputHistoryService()
        {
            history = new LinkedList<HistoryItem>();
        }

        public int Count => history.Count;

        public void Enqueue(Input input)
        {
            HistoryItem newItem = new HistoryItem(input);
            newItem.LinkWithNode(history.AddLast(newItem));
            ShrinkHistory();
        }

        public IHistoryItem GetLastInput()
        {
            return history.Last?.Value;
        }

        public IImmutableList<IHistoryItem> GetHistory()
        {
            return ((IEnumerable<IHistoryItem>)history).ToImmutableList();
        }

        private void ShrinkHistory()
        {
            while (history.Count > HISTORY_CAPACITY)
            {
                HistoryItem oldestItem = history.First.Value;
                oldestItem.SetAsDeleted();
                history.RemoveFirst();
            }

        }
    }
}
