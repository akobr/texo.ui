using System;
using System.Collections.Generic;
using BeaverSoft.Texo.Core.Inputting;

namespace BeaverSoft.Texo.Fallback.PowerShell.Transforming
{
    public class OutputModel
    {
        public readonly Input Input;
        public readonly HashSet<string> Flags;
        public readonly Dictionary<string, object> Properties;

        public bool NoNewLine;
        public string Output;

        public OutputModel(string text, InputModel input)
        {
            Output = text;
            Input = input.Input;
            Flags = input.Flags;
            Properties = input.Properties;
        }
    }
}
