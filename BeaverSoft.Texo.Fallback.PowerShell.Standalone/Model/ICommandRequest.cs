using System;

namespace BeaverSoft.Texo.Fallback.PowerShell.Core
{
    public interface ICommandRequest
    {
        Guid Key { get; }

        string Input { get; }

        bool IsCanceled { get; }

        void Cancel();
    }
}
