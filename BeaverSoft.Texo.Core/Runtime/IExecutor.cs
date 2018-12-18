using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Runtime
{
    public interface IExecutor
    {
        Input.Input PreProcess(string input, int cursorPosition);

        void Process(string input);

        void ExecuteAction(string actionUrl);

        void ExecuteAction(string actionName, IDictionary<string, string> arguments);
    }
}