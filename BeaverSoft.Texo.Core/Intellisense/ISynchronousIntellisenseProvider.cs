using BeaverSoft.Texo.Core.View;
using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Intellisense
{
    public interface ISynchronousIntellisenseProvider
    {
        IEnumerable<IItem> GetHelp(Input.Input input);
    }
}
