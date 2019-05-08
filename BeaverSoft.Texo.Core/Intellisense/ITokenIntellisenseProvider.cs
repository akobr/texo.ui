using System.Collections.Generic;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Intellisense
{
    public interface ITokenIntellisenseProvider
    {
        IEnumerable<IItem> Help(string token);
    }
}
