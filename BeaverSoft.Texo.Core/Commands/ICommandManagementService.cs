using System;

namespace BeaverSoft.Texo.Core.Commands
{
    public interface ICommandManagementService : IDisposable
    {
        bool HasCommand(string commandKey);

        object BuildCommand(string commandKey);
    }
}