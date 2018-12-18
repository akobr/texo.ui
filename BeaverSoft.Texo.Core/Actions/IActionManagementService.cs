using System;
using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Actions
{
    public interface IActionManagementService
    {
        void Register<TAction>(string actionName)
            where TAction : IAction;

        void Unregister(string actionName);

        void Execute(string actionUrl);

        void Execute(string actionName, IDictionary<string, string> arguments);
    }
}