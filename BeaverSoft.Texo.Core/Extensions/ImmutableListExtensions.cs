using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Extensions
{
    public static class ImmutableListExtensions
    {
        public static bool IsNullOrEmpty<TItem>(this IImmutableList<TItem> list)
        {
            return list == null || list.Count < 1;
        }

        public static IImmutableList<TItem> RemoveAll<TItem>(
            this IImmutableList<TItem> source,
            IImmutableList<TItem> itemsToRemove)
        {
            if (source.IsNullOrEmpty())
            {
                return ImmutableList<TItem>.Empty;
            }

            if (itemsToRemove.IsNullOrEmpty())
            {
                return source;
            }

            var builder = ImmutableList<TItem>.Empty.ToBuilder();
            var removeMap = itemsToRemove.ToImmutableHashSet();

            foreach (TItem item in source)
            {
                if(!removeMap.Contains(item))
                {
                    builder.Add(item);
                }
            }

            return builder.ToImmutable();
        }
    }
}
