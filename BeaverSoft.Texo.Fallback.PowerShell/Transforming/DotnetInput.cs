using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.Transforming;

namespace BeaverSoft.Texo.Fallback.PowerShell.Transforming
{
    public class DotnetInput : ITransformation<InputModel>
    {
        private readonly HashSet<string> targetCommands;

        public DotnetInput()
        {
            targetCommands = new HashSet<string>(
                new[] { "clean", "restore", "build", "msbuild", "build-server", "run", "test", "vstest", "pack", "publish" },
                StringComparer.OrdinalIgnoreCase);
        }

        public Task<InputModel> ProcessAsync(InputModel data)
        {
            if (string.Equals(data.Input.ParsedInput.GetFirstToken(), "dotnet", StringComparison.OrdinalIgnoreCase)
                && data.Input.ParsedInput.HasMultipleTokens
                && targetCommands.Contains(data.Input.ParsedInput.Tokens[1]))
            {
                data.Command += " /clp:ForceConsoleColor";
            }

            return Task.FromResult(data);
        }
    }
}
