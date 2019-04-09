using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Actions
{
    public interface IActionContext
    {
        string Name { get; }

        IDictionary<string, string> Arguments { get; }
    }
}