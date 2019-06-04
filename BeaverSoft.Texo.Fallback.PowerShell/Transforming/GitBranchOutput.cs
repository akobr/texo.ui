using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Text;
using BeaverSoft.Texo.Core.Transforming;

namespace BeaverSoft.Texo.Fallback.PowerShell.Transforming
{
    public class GitBranchOutput : ITransformation<OutputModel>
    {
        private readonly Regex branchNameRegex = new Regex("[^\\s\\u001b]{3,}", RegexOptions.Compiled);

        public Task<OutputModel> ProcessAsync(OutputModel data)
        {
            if (!data.Flags.Contains(TransformationFlags.GIT_BRANCH))
            {
                return Task.FromResult(data);
            }

            string branchName = data.Output.Trim();

            if (branchName.StartsWith("\u001b")
                || branchName.Contains(' '))
            {
                return Task.FromResult(data);
            }

            data.Output = branchNameRegex.Replace(data.Output, ReplaceBranchName);
            return Task.FromResult(data);
        }

        private string ReplaceBranchName(Match match)
        {
            AnsiStringBuilder builder = new AnsiStringBuilder();
            string helpCommand = $"git checkout {match.Value}";
            builder.AppendLink(match.Value, ActionBuilder.InputSetUri(helpCommand));
            return builder.ToString();
        }
    }
}
