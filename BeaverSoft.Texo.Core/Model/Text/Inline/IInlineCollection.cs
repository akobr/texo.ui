using System.Collections.Generic;
using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public interface IInlineCollection : IInline, IEnumerable<IInline>
    {
        IImmutableList<IInline> Children { get; }
    }
}