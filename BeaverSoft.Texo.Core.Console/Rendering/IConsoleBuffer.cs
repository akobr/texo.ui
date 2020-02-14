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
        /// Gets area of the buffer, position and size against original buffer.
        /// The size is with control column(s).
        /// </summary>
        Rectangle Area { get; }

        /// <summary>
        /// Gets size of the buffer, with control column(s).
        /// </summary>
        Size Size { get; }

        IReadOnlyList<GraphicAttributes> Styles { get; }

        ConsoleBufferChangeBatch Changes { get; }

        /// <summary>
        /// Gets size of the screen, without control column(s).
        /// </summary>
        Rectangle Screen { get; }

        Point Cursor { get; }
    }
}