using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Text;
using BeaverSoft.Texo.Core.Transforming;

namespace BeaverSoft.Texo.Fallback.PowerShell.Transforming
{
    public class OutputAutoUrl : ITransformationWithControl<OutputModel>
    {
        private readonly Regex urlExpression = new Regex("https?:\\/\\/(www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{2,256}\\.[a-z]{2,4}\b([-a-zA-Z0-9@:%_\\+.~#?&//=]*)", RegexOptions.Compiled);

        public async Task<OutputModel> ProcessAsync(OutputModel data, Func<OutputModel, Task<OutputModel>> nextFlow)
        {
            await ProcessAsync(data);

            if (data.Flags.Contains(TransformationFlags.AUTO_URL))
            {
                return data;
            }

            return await nextFlow(data);
        }

        public Task<OutputModel> ProcessAsync(OutputModel data)
        {
            int originalLength = data.Output.Length;
            data.Output = urlExpression.Replace(data.Output, UrlReplacement);

            if (data.Output.Length != originalLength)
            {
                data.Flags.Add(TransformationFlags.AUTO_URL);
            }

            return Task.FromResult(data);
        }

        private string UrlReplacement(Match match)
        {
            AnsiStringBuilder builder = new AnsiStringBuilder();
            builder.AppendLink(match.Value, match.Value);
            return builder.ToString();
        }
    }
}
