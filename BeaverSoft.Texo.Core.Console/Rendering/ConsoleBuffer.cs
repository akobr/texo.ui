using System;
using System.Collections.Generic;
using System.Drawing;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public class ConsoleBuffer : IConsoleBuffer
    {
        private readonly ReadOnlyMemory<BufferCell> buffer;

        public ConsoleBuffer(
            ReadOnlyMemory<BufferCell> buffer,
            ConsoleBufferChangeBatch changes,
            IReadOnlyList<GraphicAttributes> styles)
        {
            this.buffer = buffer;
            Changes = changes;
            Styles = styles;
            Screen = changes.EndScreen;
            Cursor = changes.EndCursor;
        }

        public BufferCell this[int rowIndex, int columnIndex]
            => buffer.Span[rowIndex * Screen.Width + columnIndex];

        public Rectangle Screen { get; }

        public Point Cursor { get; }

        public ConsoleBufferChangeBatch Changes { get; }

        public IReadOnlyList<GraphicAttributes> Styles { get; }

        public ReadOnlySpan<BufferCell> GetScreen()
        {
            return buffer.Slice(Screen.Y * Screen.Width + Screen.X, Screen.Height * Screen.Width).Span;
        }

        public ReadOnlySpan<BufferCell> GetFull()
        {
            return buffer.Span;
        }
    }
}
