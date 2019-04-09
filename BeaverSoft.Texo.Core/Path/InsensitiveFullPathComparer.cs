using System;
using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Path
{
    public class InsensitiveFullPathComparer : IComparer<string>, IEqualityComparer<string>
    {
        public int Compare(string x, string y)
        {
            return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals(string x, string y)
        {
            return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(string path)
        {
            return path.GetFullConsolidatedPath().GetHashCode();
        }
    }
}
