using System.Drawing;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public struct ConsoleBufferChange
    {
        public readonly Point Start;

        public readonly Point End;

        public ConsoleBufferChange(Point start, Point end)
        {
            Start = start;
            End = end;
        }
    }
}
