using System;

namespace BeaverSoft.Texo.Fallback.PowerShell.Core
{
    public class CommandRequest : ICommandRequest
    {
        public CommandRequest(Guid key, string input)
        {
            Key = key;
            Input = input;
        }

        public Guid Key { get; }

        public string Input { get; }

        public bool IsCanceled { get; private set; }

        public void Cancel()
        {
            IsCanceled = true;
        }
    }
}
