using System;
using System.Collections.Generic;

namespace Commands.CodeBaseSearch.Extensions
{
    public static class ListExtensions
    {
        public static IEnumerable<List<T>> Split<T>(this List<T> list, int chunksCount)
        {
            int chunkSize = list.Count / chunksCount;

            if (list.Count % chunksCount > 0)
            {
                chunkSize++;
            }

            for (int i = 0; i < list.Count; i += chunkSize)
            {
                yield return list.GetRange(i, Math.Min(chunkSize, list.Count - i));
            }
        }
    }
}
