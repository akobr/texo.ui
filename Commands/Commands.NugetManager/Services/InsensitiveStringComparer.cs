using System;
using System.Collections.Generic;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public class InsensitiveStringComparer : IComparer<string>, IEqualityComparer<string>
    {
        public int Compare(string x, string y)
        {
            return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals(string x, string y)
        {
            return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(string obj)
        {
            return string.IsNullOrEmpty(obj)
                ? 0
                : obj.ToLowerInvariant().GetHashCode();
        }
    }
}
