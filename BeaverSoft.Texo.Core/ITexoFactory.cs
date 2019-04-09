using System;

namespace BeaverSoft.Texo.Core
{
    public interface ITexoFactory<TItem>
    {
        TItem Create(Type itemType);

        void Release(TItem item);
    }

    public interface ITexoFactory<TItem, in TContext>
    {
        TItem Create(TContext context);

        void Release(TItem item);
    }
}