using System.Buffers;
using System.Collections.Generic;
using System.Drawing;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public class ConsoleBuffer : IConsoleBuffer
    {
        public const int CONTROL_LENGTH = 1;

        private readonly ArrayPool<BufferCell> bufferPool;
        private readonly BufferCell[] buffer;
        private readonly int widthWithControl;

        public ConsoleBuffer(
            ConsoleBufferType type,
            BufferCell[] buffer,
            ConsoleBufferChangeBatch changes,
            IReadOnlyList<GraphicAttributes> styles,
            ArrayPool<BufferCell> bufferPool)
        {
            this.buffer = buffer;
            this.bufferPool = bufferPool;
            Type = type;
            Changes = changes;
            Styles = styles;
            Area = changes.Area;
            Screen = changes.EndScreen;
            Cursor = changes.EndCursor;
            widthWithControl = Screen.Width + CONTROL_LENGTH;
        }

        public BufferCell this[int columnIndex, int rowIndex]
            => buffer[rowIndex * widthWithControl + columnIndex];

        public BufferCell this[int flatIndex]
            => buffer[flatIndex];

        public ConsoleBufferType Type { get; }

        public Rectangle Area { get; }

        public Size Size => Area.Size;

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
