using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.Transforming;

namespace BeaverSoft.Texo.Fallback.PowerShell.Transforming
{
    public class GetChildItemInput : ITransformation<InputModel>
    {
        private readonly HashSet<string> targetCommands;

        public GetChildItemInput()
        {
            targetCommands = new HashSet<string>(
                new[] { "ls", "dir", "gci", "get-childitem" },
                StringComparer.OrdinalIgnoreCase);
        }

        public Task<InputModel> ProcessAsync(InputModel data)
        {
            if (targetCommands.Contains(data.Input.ParsedInput.GetFirstToken()))
            {
                data.Flags.Add(TransformationFlags.GET_CHILD_ITEM);

                if (data.Input.ParsedInput.Tokens.Contains("-name"))
                {
                    data.Flags.Add(TransformationFlags.GET_CHILD_ITEM_NAME);
                }
            }

            return Task.FromResult(data);
        }
    }
}
