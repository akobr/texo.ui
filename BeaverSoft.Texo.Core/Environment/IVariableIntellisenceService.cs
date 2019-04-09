using BeaverSoft.Texo.Core.View;
using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Environment
{
    public interface IVariableIntellisenceService
    {
        IEnumerable<IItem> Help(string input);
    }
}
