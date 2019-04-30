using BeaverSoft.Texo.Core.View;
using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Commands
{
    public interface ISimpleIntellisenseSource
    {
        IEnumerable<IItem> GetHelp(string input);
    }
}
