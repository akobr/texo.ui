using System;
using System.Collections.Generic;
using System.Drawing;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public interface IConsoleBuffer : IDisposable
    {
        BufferCell this[int columnIndex, int rowIndex] { get; }

        ConsoleBufferType Type { get; }

        /// <summary>
        /// Gets size of the buffer, with control column.
        /// </summary>
        Size Size { get; }

        IReadOnlyList<GraphicAttributes> Styles { get; }

        ConsoleBufferChangeBatch Changes { get; }

        /// <summary>
        /// Gets size of the screen, without control column.
        /// </summary>
        Rectangle Screen { get; }

        Point Cursor { get; }
    }
}