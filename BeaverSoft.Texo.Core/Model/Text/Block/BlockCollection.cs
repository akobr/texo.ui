using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Model.Text.Block
{
    public class BlockCollection : IBlockCollection
    {
        private ImmutableList<IBlock> children;

        public IImmutableList<IBlock> Children => children;

        public void AddBlock(IBlock block)
        {
            children.Add(block);
        }

        public IEnumerator<IBlock> GetEnumerator()
        {
            return children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return children.GetEnumerator();
        }
    }
}
