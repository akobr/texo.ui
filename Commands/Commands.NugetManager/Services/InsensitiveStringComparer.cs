using System;
using System.Collections.Generic;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public class InsensitiveStringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
        }
    }
}
