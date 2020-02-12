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
        private readonly int controlLength;
        private int startScreenIndex, startScreenLenght, startScreenLineWidth, startCursor;
        private int startIndex, endIndex;

        public BitArrayChangesManager(int bufferLength, int controlLength)
        {
            changeMask = new BitArray(bufferLength);
            this.controlLength = controlLength;
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

        public void Start(int startScreenIndex, int startScreenLenght, int startScreenLineWidth, int startCursor)
        {
            Reset();

            this.startScreenIndex = startScreenIndex;
            this.startScreenLenght = startScreenLenght;
            this.startScreenLineWidth = startScreenLineWidth;
            this.startCursor = startCursor;
        }

        public ConsoleBufferChangeBatch Finish(ConsoleBufferType batchType, int endScreenIndex, int endScreenLenght, int endScreenLineWidth, int endCursor)
        {
            int startLineFullWidth = startScreenLineWidth + controlLength;
            int endLineFullWidth = endScreenLineWidth + controlLength;

            int startWindowIndex, endWindowIndex;

            switch (batchType)
            {
                case ConsoleBufferType.AllChanges:
                    startWindowIndex = Math.Min(startIndex, endScreenIndex);
                    endWindowIndex = Math.Max(endIndex, endScreenIndex + endScreenLenght - 1);
                    break;

                case ConsoleBufferType.Full:
                    startWindowIndex = 0;
                    endWindowIndex = changeMask.Length - 1;
                    break;

                // case ConsoleBufferType.Screen:
                default:
                    startWindowIndex = endScreenIndex;
                    endWindowIndex = endScreenIndex + endScreenLenght - 1;
                    break;
            }

            Rectangle startScreen = new Rectangle(startScreenIndex % startLineFullWidth, startScreenIndex / startLineFullWidth, startScreenLineWidth, startScreenLenght / startLineFullWidth);
            Rectangle endScreen = new Rectangle(endScreenIndex % endLineFullWidth, endScreenIndex / endLineFullWidth, endScreenLineWidth, endScreenLenght / endLineFullWidth);
            Point startCursorPoint = new Point((startCursor - startScreenIndex) % startLineFullWidth, (startCursor - startScreenIndex) / startLineFullWidth);
            Point endCursorPoint = new Point((endCursor - endScreenIndex) % endLineFullWidth, (endCursor - endScreenIndex) / endLineFullWidth);
            return new ConsoleBufferChangeBatch(startScreen, endScreen, startCursorPoint, endCursorPoint, BuildSequences(startWindowIndex, endWindowIndex, endLineFullWidth).ToImmutableList());
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
                            new Point((sequenceStart - startWindowIndex) % lineWidth, (sequenceStart - startWindowIndex) / lineWidth),
                            new Point((sequenceEnd - startWindowIndex) % lineWidth, (sequenceEnd - startWindowIndex) / lineWidth));

                    isSequenceInProgress = false;
                }
            }

            if (isSequenceInProgress)
            {
                yield return new ConsoleBufferChange(
                    new Point((sequenceStart - startWindowIndex) % lineWidth, (sequenceStart - startWindowIndex) / lineWidth),
                    new Point((endIndex - startWindowIndex) % lineWidth, (endIndex - startWindowIndex) / lineWidth));
            }
        }
    }
}
