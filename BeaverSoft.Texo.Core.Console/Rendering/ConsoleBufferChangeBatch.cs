using System.Collections.Generic;
using System.Drawing;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public class ConsoleBufferChangeBatch
    {
        public ConsoleBufferChangeBatch(
            Rectangle area,
            Rectangle startScreen,
            Rectangle endScreen,
            Point startCursor,
            Point endCursor,
            IReadOnlyCollection<ConsoleBufferChange> changes)
        {
            Area = area;
            StartScreen = startScreen;
            EndScreen = endScreen;
            StartCursor = startCursor;
            EndCursor = endCursor;
            Changes = changes;
        }

        public Rectangle Area { get; }

        public Rectangle StartScreen { get; }

        public Rectangle EndScreen { get; }

        public Point StartCursor { get; }

        public Point EndCursor { get; }

        public IReadOnlyCollection<ConsoleBufferChange> Changes { get; }
    }
}
