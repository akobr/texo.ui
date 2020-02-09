using System.Buffers;
using System.Collections.Generic;
using System.Drawing;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public class ConsoleBuffer : IConsoleBuffer
    {
        private readonly ArrayPool<BufferCell> bufferPool;
        private readonly BufferCell[] buffer;
        private readonly int widthWithControl;

        public ConsoleBuffer(
            BufferCell[] buffer,
            int bufferUsedSize,
            ArrayPool<BufferCell> bufferPool,
            ConsoleBufferChangeBatch changes,
            IReadOnlyList<GraphicAttributes> styles,
            ConsoleBufferType type)
        {
            this.buffer = buffer;
            this.bufferPool = bufferPool;
            Type = type;

            Changes = changes;
            Styles = styles;
            Screen = changes.EndScreen;
            Cursor = changes.EndCursor;

            widthWithControl = Screen.Width + ConsoleBufferBuilder.CONTROL_LENGTH;
            Size = new Size(widthWithControl, bufferUsedSize / widthWithControl);
        }

        public BufferCell this[int columnIndex, int rowIndex]
            => buffer[rowIndex * widthWithControl + columnIndex];

        public ConsoleBufferType Type { get; }

        public Size Size { get; }

        public Rectangle Screen { get; }

        public Point Cursor { get; }

        public ConsoleBufferChangeBatch Changes { get; }

        public IReadOnlyList<GraphicAttributes> Styles { get; }

        public void Dispose()
        {
            bufferPool.Return(buffer);
        }
    }
}
