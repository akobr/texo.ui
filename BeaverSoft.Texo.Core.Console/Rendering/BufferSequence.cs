namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public struct BufferSequence
    {
        public readonly int StartIndex;

        public readonly int EndIndex;

        public BufferSequence(int start, int end)
        {
            StartIndex = start;
            EndIndex = end;
        }

        public bool IsValid => StartIndex < EndIndex;

        public bool IsEndingBeforeOrSame(BufferSequence other)
        {
            return EndIndex <= other.EndIndex;
        }
    }
}
