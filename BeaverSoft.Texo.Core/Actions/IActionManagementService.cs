using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Actions
{
    public interface IActionManagementService
    {
        Task ExecuteAsync(string actionUrl);

        Task ExecuteAsync(string actionName, IDictionary<string, string> arguments);
    }
}