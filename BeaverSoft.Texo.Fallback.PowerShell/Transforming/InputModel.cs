using System;
using System.Collections.Generic;
using BeaverSoft.Texo.Core.Inputting;

namespace BeaverSoft.Texo.Fallback.PowerShell.Transforming
{
    public class InputModel
    {
        public readonly Input Input;
        public readonly HashSet<string> Flags;
        public readonly Dictionary<string, object> Properties;

        public string Command;

        public InputModel(Input input)
        {
            Input = input;
            Command = input.ParsedInput.RawInput;
            Flags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
