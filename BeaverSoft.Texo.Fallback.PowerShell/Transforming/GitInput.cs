using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Transforming;

namespace BeaverSoft.Texo.Fallback.PowerShell.Transforming
{
    public class GitInput : ITransformation<InputModel>
    {
        private readonly HashSet<string> targetCommands;
        private const string PROGRESS_ARGUMENT = " --progress";

        public GitInput()
        {
            targetCommands = new HashSet<string>(
                new[] { "push", "pull", "fetch", "clone" },
                StringComparer.OrdinalIgnoreCase);
        }

        public Task<InputModel> ProcessAsync(InputModel data)
        {
            if (!string.Equals(data.Input.ParsedInput.Tokens[0], "git", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(data);
            }

            data.Command = "git -c color.ui=always -c color.status=always -c color.branch=always -c color.diff=always -c color.interactive=always ";
            data.Command += string.Join(" ", data.Input.ParsedInput.Tokens.Skip(1));

            if (data.Input.ParsedInput.HasMultipleTokens)
            {
                string command = data.Input.ParsedInput.Tokens[1];

                if (string.Equals(command, "status", StringComparison.OrdinalIgnoreCase))
                {
                    data.Flags.Add(TransformationFlags.GIT_STATUS);
                }
                else if(string.Equals(command, "checkout", StringComparison.OrdinalIgnoreCase))
                {
                    int pathSpecIndex = data.Command.IndexOf(" -- ");

                    if (pathSpecIndex > 0)
                    {
                        data.Command = data.Command.Insert(pathSpecIndex, PROGRESS_ARGUMENT);
                    }
                    else
                    {
                        data.Command += PROGRESS_ARGUMENT;
                    }
                }
                else if (targetCommands.Contains(command))
                {
                    data.Command += PROGRESS_ARGUMENT;
                }

                if (string.Equals(command, "push", StringComparison.OrdinalIgnoreCase))
                {
                    data.Flags.Add(TransformationFlags.GIT_PUSH);
                }
                else if(string.Equals(command, "branch", StringComparison.OrdinalIgnoreCase))
                {
                    data.Flags.Add(TransformationFlags.GIT_BRANCH);
                }
            }

            data.Flags.Add(TransformationFlags.GIT);
            return Task.FromResult(data);
        }
    }
}
