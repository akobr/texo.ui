using System.Collections.Generic;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Intellisense
{
    public interface IIntellisenseCache
    {
        void AddOrSet(string input, IEnumerable<IItem> items);

        void Clear();

        void Remove(string input);

        void Set(string input, IEnumerable<IItem> items);

        bool TryGet(string input, out IEnumerable<IItem> items);
    }
}