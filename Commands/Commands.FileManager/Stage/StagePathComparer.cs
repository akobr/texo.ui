using System;
using System.Collections.Generic;

namespace BeaverSoft.Texo.Commands.FileManager.Stage
{
    public class StagePathComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
        }
    }
}
