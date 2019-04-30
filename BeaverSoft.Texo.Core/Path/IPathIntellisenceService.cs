using BeaverSoft.Texo.Core.View;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeaverSoft.Texo.Core.Path
{
    public interface IPathIntellisenseService
    {
        IEnumerable<IItem> Help(string currentPath);
    }
}
