using System;
using System.Collections.Generic;
using System.Drawing;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public interface IViewBuffer
    {
        ViewCell this[int rowIndex, int columnIndex] { get; }

        IReadOnlyList<GraphicAttributes> Styles { get; }

        IReadOnlyCollection<Sequence> Changes { get; }

        Rectangle Screen { get; }

        Point Cursor { get; }

        ReadOnlySpan<ViewCell> GetFull();

        ReadOnlySpan<ViewCell> GetScreen();
    }
}