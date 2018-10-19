using System.Collections.Generic;
using BeaverSoft.Texo.Core.Model.Configuration;

namespace BeaverSoft.Texo.Core.InputTree
{
    public interface IInputTreeBuilder
    {
        InputTree Build(IEnumerable<Query> commands, string defaultCommandKey);
    }
}