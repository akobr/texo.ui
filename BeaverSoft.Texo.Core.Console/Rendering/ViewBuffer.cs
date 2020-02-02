using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public class ViewBuffer : IEnumerable<ViewCell>
    {
        private readonly ImmutableArray<ViewCell> buffer;
        private readonly int columnCount;

        public ViewBuffer(ImmutableArray<ViewCell> buffer, int columnCount)
        {
            this.columnCount = columnCount;
            this.buffer = buffer;
        }

        public ViewCell this[int rowIndex, int columnIndex]
            => buffer[rowIndex * columnCount + columnIndex];

        public IEnumerator<ViewCell> GetEnumerator()
        {
            return ((IEnumerable<ViewCell>)buffer).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)buffer).GetEnumerator();
        }
    }
}
