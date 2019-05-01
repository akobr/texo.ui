using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Inputting.History
{
    public class HistoryItem : IHistoryItem
    {
        private LinkedListNode<HistoryItem> node;
        private HistoryItem deletedFrom;

        public HistoryItem(Input input)
        {
            Input = input;
        }

        public Input Input { get; }

        public HistoryItem Previous => node.Previous?.Value;

        public HistoryItem Next => node.Next?.Value ?? deletedFrom;

        public bool HasPrevious => node.Previous != null;

        public bool HasNext => node.Next != null;

        public bool IsDeleted => node.List == null;

        internal void LinkWithNode(LinkedListNode<HistoryItem> nodeToLink)
        {
            node = nodeToLink;
        }

        internal void SetAsDeleted()
        {
            deletedFrom = Next;
        }
    }
}
