using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Core.Runtime
{
    public interface ICommandManagementService
    {
        bool HasCommand(string commandKey);

        ICommand BuildCommand(string commandKey);
    }
}