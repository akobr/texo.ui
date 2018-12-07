using System.Collections.Generic;
using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public interface IBlockCollection : IBlock, IEnumerable<IBlock>
    {
        IImmutableList<IBlock> Children { get; }
    }
}