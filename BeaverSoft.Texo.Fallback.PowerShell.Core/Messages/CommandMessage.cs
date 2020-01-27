using System;

namespace BeaverSoft.Texo.Fallback.PowerShell.Core.Messages
{
    public class CommandMessage : ICommandMessage
    {
        public CommandMessage() { }

        public CommandMessage(Guid key, MessageType type, string content = null)
        {
            Key = key;
            Type = type;
            Content = content;
        }

        public Guid Key { get; set; }

        public MessageType Type { get; set; }

        public string Content { get; set; }
    }
}
