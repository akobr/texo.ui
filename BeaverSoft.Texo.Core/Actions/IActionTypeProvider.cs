using System;

namespace BeaverSoft.Texo.Core.Actions
{
    public interface IActionTypeProvider
    {
        Type GetActionType(string actionName);
    }
}