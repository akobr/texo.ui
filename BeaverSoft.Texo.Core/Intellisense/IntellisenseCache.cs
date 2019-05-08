using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Intellisense
{
    public class IntellisenseCache : IIntellisenseCache
    {
        private readonly ConcurrentDictionary<string, ImmutableList<IItem>> cache;

        public IntellisenseCache()
            : this(StringComparer.OrdinalIgnoreCase)
        {
            // no operation
        }

        public IntellisenseCache(IEqualityComparer<string> comparer)
        {
            cache = new ConcurrentDictionary<string, ImmutableList<IItem>>(comparer);
        }

        public bool TryGet(string input, out IEnumerable<IItem> items)
        {
            bool result = cache.TryGetValue(input, out var cachedValue);
            items = cachedValue;
            return result;
        }

        public void AddOrSet(string input, IEnumerable<IItem> items)
        {
            if (cache.TryGetValue(input, out var cacheValue))
            {
                cache[input] = cacheValue.AddRange(items);
            }
            else
            {
                Set(input, items);
            }
        }

        public void Set(string input, IEnumerable<IItem> items)
        {
            cache[input] = ImmutableList<IItem>.Empty.AddRange(items);
        }

        public void Remove(string input)
        {
            cache.TryRemove(input, out _);
        }

        public void Clear()
        {
            cache.Clear();
        }
    }
}
