using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;

namespace BeaverSoft.Texo.Core.Console.Rendering.Managers
{
    class BitArrayChangesManager : IConsoleBufferChangesManager
    {
        private readonly BitArray changes;
        private int screenStart, screenLenght, lineWidth, cursor;
        private int startIndex, endIndex;

        public BitArrayChangesManager(int length)
        {
            changes = new BitArray(length);
        }

        public void AddChange(int index)
        {
            if (startIndex > index) startIndex = index;
            if (endIndex < index) startIndex = index;

            changes.Set(index, true);
        }

        public void AddChange(int start, int length)
        {
            if (startIndex > start) startIndex = start;
            if (endIndex < start + length - 1) endIndex = start + length - 1;

            for (int i = start; i < length; i++)
            {
                changes.Set(i, true);
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

        public ConsoleBufferChangeBatch Finish(int screenStart, int screenLenght, int lineWidth, int cursor)
        {
            Rectangle startScreen = new Rectangle(this.screenStart % this.lineWidth, this.screenStart / this.lineWidth, this.lineWidth, this.screenLenght / this.lineWidth);
            Rectangle endScreen = new Rectangle(screenStart % lineWidth, screenStart / lineWidth, lineWidth, screenLenght / lineWidth);
            Point startCursor = new Point(this.cursor % this.lineWidth, this.cursor / this.lineWidth);
            Point endCursor = new Point(cursor % lineWidth, cursor / lineWidth);
            return new ConsoleBufferChangeBatch(startScreen, endScreen, startCursor, endCursor, BuildSequences());
        }

        public void Reset()
        {
            startIndex = int.MaxValue;
            endIndex = int.MinValue;
            changes.SetAll(false);
        }

        private IReadOnlyCollection<Sequence> BuildSequences()
        {
            if (startIndex == int.MaxValue)
            {
                return ImmutableList<Sequence>.Empty;
            }

            var sequenceBuilder = ImmutableList.CreateBuilder<Sequence>();
            bool isSequenceInProgress = false;
            int sequenceStart = -1;

            for (int i = startIndex; i <= endIndex; ++i)
            {
                bool bit = changes.Get(i);

                if (bit && !isSequenceInProgress)
                {
                    isSequenceInProgress = true;
                    sequenceStart = i;
                }
                else if (!bit && isSequenceInProgress)
                {
                    sequenceBuilder.Add(new Sequence(sequenceStart, i - 1));
                    isSequenceInProgress = false;
                }
            }

            if (isSequenceInProgress)
            {
                sequenceBuilder.Add(new Sequence(sequenceStart, endIndex));
            }

            return sequenceBuilder.ToImmutable();
        }
    }
}
