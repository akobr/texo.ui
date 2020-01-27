using System;

namespace BeaverSoft.Texo.Fallback.PowerShell.Core.Messages
{
    public interface ICommandMessage
    {
        Guid Key { get; }

        MessageType Type { get; }

        string Content { get; }
    }
}