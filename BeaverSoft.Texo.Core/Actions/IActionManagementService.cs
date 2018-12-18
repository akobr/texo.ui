using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Actions
{
    public interface IActionManagementService
    {
        void Execute(string actionUrl);

        void Execute(string actionName, IDictionary<string, string> arguments);
    }
}