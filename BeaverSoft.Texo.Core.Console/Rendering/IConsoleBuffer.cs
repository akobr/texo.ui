using System;
using System.Collections.Generic;
using System.Drawing;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public interface IConsoleBuffer
    {
        BufferCell this[int rowIndex, int columnIndex] { get; }

        IReadOnlyList<GraphicAttributes> Styles { get; }

        ConsoleBufferChangeBatch Changes { get; }

        Rectangle Screen { get; }

        Point Cursor { get; }

        ReadOnlySpan<BufferCell> GetFull();

        ReadOnlySpan<BufferCell> GetScreen();
    }
}