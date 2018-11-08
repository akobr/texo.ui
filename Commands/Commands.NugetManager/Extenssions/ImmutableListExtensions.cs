using System.Collections.Immutable;

namespace BeaverSoft.Texo.Commands.NugetManager.Extenssions
{
    public static class ImmutableListExtensions
    {
        public static bool IsNullOrEmpty<TItem>(this IImmutableList<TItem> list)
        {
            return list == null || list.Count < 1;
        }
    }
}
