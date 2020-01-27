using System;
using System.Threading.Tasks;
using BeaverSoft.Texo.Fallback.PowerShell.Core;
using StrongBeaver.Core;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.Services
{
    public interface IPowerShellService : IInitialisable, IDisposable
    {
        Guid CommandInProgressKey { get; }

        bool IsCommandInProgress { get; }

        Task LastCommandExecutionTask { get; }

        void AddCommandToQueue(ICommandRequest command);

        void CancelCommand(Guid commandKey);
    }
}