using System;
using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    // TODO: should be a binary tree which is splitting the buffer
    public class ViewSequenceChanges : IViewChangesManager
    {
        private readonly LinkedList<Sequence> changes;
        private int sequenceInProgressStart, sequenceInProgressEnd;

        public ViewSequenceChanges()
        {
            changes = new LinkedList<Sequence>();
        }

        public void AddChange(int index)
        {
            if (sequenceInProgressStart < 0)
            {
                sequenceInProgressStart = sequenceInProgressEnd = index;
                return;
            }

            if (sequenceInProgressEnd == index - 1)
            {
                sequenceInProgressEnd = index;
                return;
            }

            AddChange(sequenceInProgressStart, sequenceInProgressEnd);
            sequenceInProgressStart = sequenceInProgressEnd = index;
        }

        public void AddChange(int start, int end)
        {
            AddChange(new Sequence(start, end));
        }

        public void AddChange(Sequence sequence)
        {
            if (!sequence.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(sequence), "Invalid change sequence; start must be smaller or equal to end.");
            }

            LinkedListNode<Sequence> listItem = changes.First;
            LinkedListNode<Sequence> afterItem = null, beforeItem = null;

            while (listItem != null)
            {
                if (IsConnected(sequence, listItem.Value))
                {
                    sequence = Union(sequence, listItem.Value);
                }
                else if (sequence.IsBefore(listItem.Value))
                {
                    beforeItem = listItem;
                    break;
                }
                else
                {
                    afterItem = listItem;
                }
            }

            if (afterItem != null)
            {
                LinkedListNode<Sequence> newChange = changes.AddAfter(afterItem, sequence);

                if (beforeItem != null)
                {
                    changes.AddBefore(beforeItem, newChange);
                }
            }
            else if (beforeItem != null)
            {
                changes.AddBefore(beforeItem, sequence);
            }
            else
            {
                changes.AddLast(sequence);
            }
        }

        public void Reset()
        {
            changes.Clear();
        }

        private bool IsConnected(Sequence first, Sequence second)
        {
            return first.StartIndex <= second.EndIndex + 1
                && first.EndIndex >= second.StartIndex - 1;
        }

        private Sequence Union(Sequence first, Sequence second)
        {
            return new Sequence(
                first.StartIndex < second.StartIndex ? first.StartIndex : second.StartIndex,
                first.EndIndex > second.EndIndex ? first.EndIndex : second.EndIndex);
        }

        public struct Sequence
        {
            public readonly int StartIndex;

            public readonly int EndIndex;

            public Sequence(int start, int end)
            {
                StartIndex = start;
                EndIndex = end;
            }

            public bool IsValid()
            {
                return StartIndex < EndIndex;
            }

            public bool IsBefore(Sequence other)
            {
                return EndIndex <= other.EndIndex;
            }
        }
    }
}
