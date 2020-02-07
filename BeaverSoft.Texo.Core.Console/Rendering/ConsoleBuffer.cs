using System;
using System.Collections.Generic;
using System.Drawing;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public class ConsoleBuffer : IConsoleBuffer
    {
        private readonly ReadOnlyMemory<BufferCell> buffer;
        private readonly int controlLength;

        public ConsoleBuffer(
            ReadOnlyMemory<BufferCell> buffer,
            ConsoleBufferChangeBatch changes,
            IReadOnlyList<GraphicAttributes> styles,
            int controlLength)
        {
            this.buffer = buffer;
            this.controlLength = controlLength;

            Changes = changes;
            Styles = styles;
            Screen = changes.EndScreen;
            Cursor = changes.EndCursor;
        }

        // TODO: [P1] calculate with control columns
        public BufferCell this[int columnIndex, int rowIndex]
            => buffer.Span[rowIndex * (Screen.Width + controlLength) + columnIndex];

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
