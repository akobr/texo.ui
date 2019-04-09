using System;
using System.Collections.Generic;
using System.Text;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public class InsensitiveOpositeStringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return string.Compare(y, x, StringComparison.OrdinalIgnoreCase);
        }
    }
}
