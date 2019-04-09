using BeaverSoft.Texo.Core.View;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeaverSoft.Texo.Core.Path
{
    public interface IPathIntellisenceService
    {
        IEnumerable<IItem> Help(string currentPath);
    }
}
