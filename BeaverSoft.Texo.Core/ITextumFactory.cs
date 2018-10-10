using System;

namespace BeaverSoft.Texo.Core
{
    public interface ITextumFactory<TItem>
    {
        TItem Create(Type itemType);

        void Release(TItem item);
    }
}