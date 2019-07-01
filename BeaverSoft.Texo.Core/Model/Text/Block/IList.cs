using System.Collections.Generic;
using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public interface IList : IBlock, IEnumerable<IListItem>
    {
        IImmutableList<IListItem> Items { get; }
    }
}