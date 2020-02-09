using System;
using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Console.Rendering.Managers
{
    public partial class SequenceChangesManager : IConsoleBufferChangesManager
    {
        private readonly LinkedList<BufferSequence> changes;
        private int screenStart, screenLenght, lineWidth, cursor;
        private int sequenceInProgressStart, sequenceInProgressEnd;

        public BufferSequence AllChangeSequence
            => new BufferSequence(changes.First.Value.StartIndex, changes.Last.Value.EndIndex);

        public SequenceChangesManager()
        {
            changes = new LinkedList<BufferSequence>();
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
            AddChange(new BufferSequence(start, end));
        }

        public void AddChange(BufferSequence sequence)
        {
            if (!sequence.IsValid)
            {
                throw new ArgumentOutOfRangeException(nameof(sequence), "Invalid change sequence; start must be smaller or equal to end.");
            }

            LinkedListNode<BufferSequence> listItem = changes.First;
            LinkedListNode<BufferSequence> afterItem = null, beforeItem = null;

            while (listItem != null)
            {
                if (IsConnected(sequence, listItem.Value))
                {
                    sequence = Union(sequence, listItem.Value);
                }
                else if (sequence.IsEndingBeforeOrSame(listItem.Value))
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
                LinkedListNode<BufferSequence> newChange = changes.AddAfter(afterItem, sequence);

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

        public void Start(int screenStart, int screenLenght, int lineWidth, int cursor)
        {
            Reset();

            this.screenStart = screenStart;
            this.screenLenght = screenLenght;
            this.lineWidth = lineWidth;
            this.cursor = cursor;
        }

        public ConsoleBufferChangeBatch Finish(ConsoleBufferType batchType, int screenStart, int screenLenght, int lineWidth, int cursor)
        {
            throw new NotImplementedException();
        }

        public void Resize(int bufferLength)
        {
            // no operation
        }

        public void Reset()
        {
            changes.Clear();
        }

        private bool IsConnected(BufferSequence first, BufferSequence second)
        {
            return first.StartIndex <= second.EndIndex + 1
                && first.EndIndex >= second.StartIndex - 1;
        }

        private BufferSequence Union(BufferSequence first, BufferSequence second)
        {
            return new BufferSequence(
                first.StartIndex < second.StartIndex ? first.StartIndex : second.StartIndex,
                first.EndIndex > second.EndIndex ? first.EndIndex : second.EndIndex);
        }
    }
}
