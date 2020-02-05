using System;
using System.Collections;
using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    class BitArrayChangesManager : IViewChangesManager
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

        public IReadOnlyCollection<Sequence> Finish(int screenStart, int screenLenght, int lineWidth, int cursor)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            startIndex = int.MaxValue;
            endIndex = int.MinValue;
            changes.SetAll(false);
        }
    }
}
