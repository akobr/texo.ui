using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeaverSoft.Texo.Commands.NugetManager.Extenssions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }
    }
}
