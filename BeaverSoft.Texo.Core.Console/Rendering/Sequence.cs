namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public struct Sequence
    {
        public readonly int StartIndex;

        public readonly int EndIndex;

        public Sequence(int start, int end)
        {
            StartIndex = start;
            EndIndex = end;
        }

        public bool IsValid => StartIndex < EndIndex;

        public bool IsEndingBeforeOrSame(Sequence other)
        {
            return EndIndex <= other.EndIndex;
        }
    }
}
