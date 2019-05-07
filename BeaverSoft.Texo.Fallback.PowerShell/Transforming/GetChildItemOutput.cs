using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Path;
using BeaverSoft.Texo.Core.Transforming;

namespace BeaverSoft.Texo.Fallback.PowerShell.Transforming
{
    public class GetChildItemOutput : ITransformation<OutputModel>
    {
        public Task<OutputModel> ProcessAsync(OutputModel data)
        {
            if (!data.Flags.Contains(TransformationFlags.GET_CHILD_ITEM))
            {
                return Task.FromResult(data);
            }

            string text = data.Output;
            string itemPath;
            int index;

            if (data.Flags.Contains(TransformationFlags.GET_CHILD_ITEM_NAME))
            {
                itemPath = text.TrimEnd();
                index = 0;
            }
            else if (data.Properties.TryGetValue(TransformationProperties.INDEX, out object value))
            {
                index = (int)value;

                if (text.Length <= index)
                {
                    return Task.FromResult(data);
                }

                itemPath = text.Substring(index).TrimEnd();
            }
            else
            {
                if (text.StartsWith("Mode", StringComparison.OrdinalIgnoreCase)
                    && text.Contains("Name"))
                {
                    int indexOfName = text.LastIndexOf("Name");
                    data.Properties[TransformationProperties.INDEX] = indexOfName;
                }

                return Task.FromResult(data);
            }

            if (string.IsNullOrWhiteSpace(itemPath))
            {
                return Task.FromResult(data);
            }

            string parentPath = data.Input.ParsedInput.Tokens
                .Skip(1)
                .FirstOrDefault(t => !string.Equals(t, "-name", StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrEmpty(parentPath))
            {
                parentPath = PathConstants.RELATIVE_CURRENT_DIRECTORY;
            }

            string path = Path.Combine(parentPath, itemPath);

            if (path.GetPathType() == PathTypeEnum.NonExistent)
            {
                return Task.FromResult(data);
            }

            string fullPath = path.GetFullConsolidatedPath();
            StringBuilder builder = new StringBuilder();
            builder.Append(text.Substring(0, index));
            builder.Append("\u001b[999m");
            builder.Append(itemPath);
            builder.Append("|");
            builder.Append(ActionBuilder.PathUri(fullPath));
            builder.Append("\u001b[998m");

            int endIndex = index + itemPath.Length;

            if (endIndex < text.Length)
            {
                builder.Append(text.Substring(endIndex));
            }

            data.Output = builder.ToString();
            return Task.FromResult(data);
        }
    }
}
