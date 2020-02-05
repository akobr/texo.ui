using System.Collections.Generic;
using System.Drawing;
using BeaverSoft.Texo.Core.Console.Rendering.Extensions;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public class ConsoleBufferChangeBatch
    {
        public ConsoleBufferChangeBatch(
            Rectangle startScreen,
            Rectangle endScreen,
            Point startCursor,
            Point endCursor,
            IReadOnlyCollection<Sequence> changes)
        {
            StartScreen = startScreen;
            EndScreen = endScreen;
            StartCursor = startCursor;
            EndCursor = endCursor;
            Changes = changes;
        }

        public Rectangle StartScreen { get; }

        public Rectangle EndScreen { get; }

        public Point StartCursor { get; }

        public Point EndCursor { get; }

        public IReadOnlyCollection<Sequence> Changes { get; }

        public int ScreenScrolledBy
            => (EndScreen.GetZeroIndex() - StartScreen.GetZeroIndex()) / EndScreen.Width;

        public int CursorMovedBy
            => EndCursor.GetIndex(EndScreen.Width) - StartCursor.GetIndex(StartScreen.Width);

        public bool HasBeenScreenScrolled
            => EndScreen.Y != StartScreen.Y;

        public bool HasBeenScreenResized
            => EndScreen.Width != StartScreen.Width
            || EndScreen.Height != StartScreen.Height;

        public bool HasBeenCursorMoved
            => EndCursor.X != StartCursor.X
            || EndCursor.Y != StartCursor.Y;
    }
}
