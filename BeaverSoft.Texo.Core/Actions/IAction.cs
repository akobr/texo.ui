using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Actions
{
    public interface IAction
    {
        void Execute(IDictionary<string, string> arguments);
    }
}