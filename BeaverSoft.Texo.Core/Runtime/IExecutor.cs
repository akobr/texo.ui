using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Inputting;

namespace BeaverSoft.Texo.Core.Runtime
{
    public interface IExecutor
    {
        Input PreProcess(string input, int cursorPosition);

        Task ProcessAsync(string input, CancellationToken cancellation = default);

        Task ExecuteActionAsync(string actionUrl);

        Task ExecuteActionAsync(string actionName, IDictionary<string, string> arguments);
    }
}