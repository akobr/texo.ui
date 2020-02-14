using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;

namespace BeaverSoft.Texo.Core.Console.Rendering.Managers
{
    class BitArrayChangesManager : IConsoleBufferChangesManager
    {
        private readonly BitArray changeMask;
        private SizedBufferSequence startScreen;
        private int startCursor;
        private int startIndex, endIndex;

        public BitArrayChangesManager(int bufferLength)
        {
            changeMask = new BitArray(bufferLength);
        }

        public BufferSequence AllChangeSequence => new BufferSequence(startIndex, endIndex);

        public void AddChange(int index)
        {
            if (startIndex > index) startIndex = index;
            if (endIndex < index) endIndex = index;

            changeMask.Set(index, true);
        }

        public void AddChange(int start, int length)
        {
            int end = start + length - 1;

            if (startIndex > start) startIndex = start;
            if (endIndex < start + length - 1) endIndex = end;

            for (int i = start; i <= end; ++i)
            {
                changeMask.Set(i, true);
            }
        }

        public void Start(SizedBufferSequence screen, int cursor)
        {
            Reset();

            startScreen = screen;
            startCursor = cursor;
        }

        public ConsoleBufferChangeBatch Finish(SizedBufferSequence screen, int cursor, int snapshotStartIndex, int snapshotLength)
        {
            int snapshotEndIndex = snapshotStartIndex + snapshotLength - 1;
            int snapshotFullWidth = screen.Width + ConsoleBuffer.CONTROL_LENGTH;
            int snapshotStartRow = snapshotStartIndex / snapshotFullWidth;
            int startScreenFullWidth = startScreen.Width + ConsoleBuffer.CONTROL_LENGTH;
            Rectangle areaRect = new Rectangle(0, snapshotStartRow, snapshotFullWidth, snapshotLength / snapshotFullWidth);
            Rectangle startScreenRect = new Rectangle(0, startScreen.StartRow - snapshotStartRow, startScreen.Width, startScreen.Height);
            Rectangle endScreenRect = new Rectangle(0, screen.StartRow - snapshotStartRow, screen.Width, screen.Height);
            Point startCursorPoint = new Point(startCursor % startScreenFullWidth, (startCursor / startScreenFullWidth) - snapshotStartRow);
            Point endCursorPoint = new Point(cursor % snapshotFullWidth, (cursor / snapshotFullWidth) - snapshotStartRow);

            return new ConsoleBufferChangeBatch(
                areaRect,
                startScreenRect,
                endScreenRect,
                startCursorPoint,
                endCursorPoint,
                BuildSequences(snapshotStartIndex, snapshotEndIndex, snapshotFullWidth)
                    .ToImmutableList());
        }

        public void Reset()
        {
            startIndex = int.MaxValue;
            endIndex = int.MinValue;
            changeMask.SetAll(false);
        }

        public void Resize(int bufferLength)
        {
            if (changeMask.Length > bufferLength)
            {
                return;
            }

            changeMask.Length = bufferLength;
        }

        private IEnumerable<ConsoleBufferChange> BuildSequences(int startWindowIndex, int endWindowIndex, int lineWidth)
        {
            if (startIndex == int.MaxValue)
            {
                yield break;
            }

            bool isSequenceInProgress = false;
            int sequenceStart = -1;
            int startRow = startWindowIndex / lineWidth;

            for (int i = startIndex; i <= endIndex; ++i)
            {
                bool bit = changeMask.Get(i);

                if (bit && !isSequenceInProgress)
                {
                    isSequenceInProgress = true;
                    sequenceStart = i;

                    if (sequenceStart > endWindowIndex)
                    {
                        yield break;
                    }
                }
                else if (!bit && isSequenceInProgress)
                {
                    int sequenceEnd = i - 1;

                    if (sequenceEnd < startWindowIndex)
                    {
                        continue;
                    }

                    yield return new ConsoleBufferChange(
                            new Point(sequenceStart % lineWidth, (sequenceStart / lineWidth) - startRow),
                            new Point(sequenceEnd % lineWidth, (sequenceEnd / lineWidth) - startRow));

                    isSequenceInProgress = false;
                }
            }

            if (isSequenceInProgress)
            {
                yield return new ConsoleBufferChange(
                    new Point(sequenceStart % lineWidth, (sequenceStart / lineWidth) - startRow),
                    new Point(endIndex % lineWidth, (endIndex / lineWidth) - startRow));
            }
        }
    }
}
