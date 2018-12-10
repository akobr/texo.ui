using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public interface IBlockCollection : IBlock, IEnumerable<IBlock>
    {
        // no member
    }
}