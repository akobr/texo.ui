using System.Linq;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Transforming;

namespace BeaverSoft.Texo.Fallback.PowerShell.Transforming
{
    public class GitErrorUpdateOutput : ITransformation<OutputModel>
    {
        public Task<OutputModel> ProcessAsync(OutputModel data)
        {
            string text = data.Output;

            if (!data.Flags.Contains(TransformationFlags.ERROR)
                || string.IsNullOrEmpty(text))
            {
                return Task.FromResult(data);
            }

            data.NoNewLine = text.Contains('\r') || text.Contains('\n');
            return Task.FromResult(data);
        }
    }
}
