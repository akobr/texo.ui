using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Text;
using BeaverSoft.Texo.Core.Transforming;

namespace BeaverSoft.Texo.Fallback.PowerShell.Transforming
{
    public class GitPushOutput : ITransformation<OutputModel>
    {
        private readonly Regex helpRegex = new Regex("git push[^\\u001b\\r\\n]*", RegexOptions.Compiled);

        public Task<OutputModel> ProcessAsync(OutputModel data)
        {
            if (!data.Flags.Contains(TransformationFlags.GIT)
                || !data.Flags.Contains(TransformationFlags.GIT_PUSH))
            {
                return Task.FromResult(data);
            }

            string text = data.Output;

            if (text.IndexOf("git push") > 0)
            {
                data.Output = helpRegex.Replace(text, ReplaceHelpCommand);
            }

            return Task.FromResult(data);
        }

        private string ReplaceHelpCommand(Match match)
        {
            string help = match.Value.TrimEnd();
            AnsiStringBuilder builder = new AnsiStringBuilder();
            builder.AppendLink(help, ActionBuilder.InputSetUri(help));
            return builder.ToString();
        }
    }
}
