using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Path;
using BeaverSoft.Texo.Core.Text;
using BeaverSoft.Texo.Core.Transforming;

namespace BeaverSoft.Texo.Fallback.PowerShell.Transforming
{
    public class GitStatusOutput : ITransformation<OutputModel>
    {
        private readonly Regex fileRegex = new Regex("(?<type>new\\sfile|modified|deleted):\\s*(?<path>\\S[^\\r\\n\\u001b]*)", RegexOptions.Compiled);
        private readonly Regex untrackedFileRegex = new Regex("\\s*\\u001b\\[\\d+m(?<path>\\S[^\\r\\n\\u001b]*)", RegexOptions.Compiled);

        public Task<OutputModel> ProcessAsync(OutputModel data)
        {
            if (!data.Flags.Contains(TransformationFlags.GIT_STATUS))
            {
                return Task.FromResult(data);
            }

            string line = data.Output;
            Match match;

            if (data.Flags.Contains(TransformationFlags.GIT_UNTRACKED))
            {
                match = untrackedFileRegex.Match(line);
            }
            else
            {
                if (line.StartsWith("Untracked files", StringComparison.OrdinalIgnoreCase))
                {
                    data.Flags.Add(TransformationFlags.GIT_UNTRACKED);
                    return Task.FromResult(data);
                }

                match = fileRegex.Match(line);
            }

            if (!match.Success)
            {
                return Task.FromResult(data);
            }

            Group fileGroup = match.Groups["path"];
            string path = fileGroup.Value;
            Func<string, bool> pathCheckFunc = path.EndsWith("/") ? (Func<string, bool>)Directory.Exists : File.Exists;
            string fullPath = Path.Combine(PathConstants.RELATIVE_CURRENT_DIRECTORY, path);

            if (!pathCheckFunc(fullPath))
            {
                return Task.FromResult(data);
            }

            fullPath = fullPath.GetFullConsolidatedPath();
            AnsiStringBuilder builder = new AnsiStringBuilder();
            builder.Append(line.Substring(0, fileGroup.Index));
            builder.AppendLink(path, ActionBuilder.PathUri(fullPath));

            int endIndex = fileGroup.Index + fileGroup.Length;

            if (endIndex < line.Length)
            {
                builder.Append(line.Substring(endIndex));
            }

            data.Output = builder.ToString();
            return Task.FromResult(data);
        }
    }
}
