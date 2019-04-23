using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Runtime
{
    public interface IExecutor
    {
        Input.Input PreProcess(string input, int cursorPosition);

        Task ProcessAsync(string input);

        void ExecuteAction(string actionUrl);

        void ExecuteAction(string actionName, IDictionary<string, string> arguments);
    }
}