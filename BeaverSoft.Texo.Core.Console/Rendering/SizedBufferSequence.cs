namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public struct SizedBufferSequence
    {
        public readonly int StartIndex;
        public readonly int EndIndex;
        public readonly int Width;

        public SizedBufferSequence(int start, int end, int width)
        {
            StartIndex = start;
            EndIndex = end;
            Width = width;
        }

        public int Height => Length / (Width + ConsoleBuffer.CONTROL_LENGTH);

        public int Length => EndIndex - StartIndex + 1;

        public int StartRow => StartIndex / (Width + ConsoleBuffer.CONTROL_LENGTH);
    }
}
