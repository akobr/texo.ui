using System;
using System.Collections.Generic;
using System.Drawing;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public class ViewBuffer : IViewBuffer
    {
        private readonly ReadOnlyMemory<ViewCell> buffer;

        public ViewBuffer(
            ReadOnlyMemory<ViewCell> buffer,
            IReadOnlyCollection<Sequence> changes,
            IReadOnlyList<GraphicAttributes> styles,
            Rectangle screen,
            Point cursor)
        {
            this.buffer = buffer;

            Changes = changes;
            Styles = styles;
            Screen = screen;
            Cursor = cursor;
        }

        public ViewCell this[int rowIndex, int columnIndex]
            => buffer.Span[rowIndex * Screen.Width + columnIndex];

        public Rectangle Screen { get; }

        public Point Cursor { get; }

        public IReadOnlyCollection<Sequence> Changes { get; }

        public IReadOnlyList<GraphicAttributes> Styles { get; }

        public ReadOnlySpan<ViewCell> GetScreen()
        {
            return buffer.Slice(Screen.Y * Screen.Width + Screen.X, Screen.Height * Screen.Width).Span;
        }

        public ReadOnlySpan<ViewCell> GetFull()
        {
            return buffer.Span;
        }
    }
}
