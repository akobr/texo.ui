using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public interface IList : IBlock, IEnumerable<IListItem>
    {
        // no member
    }
}