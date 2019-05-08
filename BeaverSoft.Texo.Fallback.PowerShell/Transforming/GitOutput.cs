using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Transforming;

namespace BeaverSoft.Texo.Fallback.PowerShell.Transforming
{
    public class GitOutput : ITransformation<OutputModel>
    {
        private readonly Regex progressRegex = new Regex(@"\s\d{1,3}%\s", RegexOptions.Compiled);

        public Task<OutputModel> ProcessAsync(OutputModel data)
        {
            if (!data.Flags.Contains(TransformationFlags.GIT))
            {
                return Task.FromResult(data);
            }

            string text = data.Output;
            StringBuilder builder = new StringBuilder();

            if (progressRegex.IsMatch(text))
            {
                data.Flags.Add(TransformationFlags.GIT_UPDATE_MODE);
                data.NoNewLine = true;
                builder.Append('\r');
                builder.Append(text);
            }
            else
            {
                if (data.Flags.Contains(TransformationFlags.GIT_UPDATE_MODE))
                {
                    builder.AppendLine();
                }

                data.Flags.Remove(TransformationFlags.GIT_UPDATE_MODE);
                builder.Append(text);
            }

            data.Output = builder.ToString();
            return Task.FromResult(data);
        }
    }
}
