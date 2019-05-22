using System;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Text;
using BeaverSoft.Texo.Core.Transforming;

namespace BeaverSoft.Texo.Fallback.PowerShell.Transforming
{
    public class GitPushOutput : ITransformation<OutputModel>
    {
        public Task<OutputModel> ProcessAsync(OutputModel data)
        {
            if (!data.Flags.Contains(TransformationFlags.GIT)
                || !data.Flags.Contains(TransformationFlags.GIT_PUSH))
            {
                return Task.FromResult(data);
            }

            string text = data.Output;
            int index = text.IndexOf("git push");

            if (index < 0)
            {
                return Task.FromResult(data);
            }

            string help = text.Substring(index).TrimEnd();
            AnsiStringBuilder builder = new AnsiStringBuilder();
            builder.Append(text.Substring(0, index));
            builder.AppendLink(help, ActionBuilder.InputSetUri(help));
            data.Output = builder.ToString();
            return Task.FromResult(data);
        }
    }
}
