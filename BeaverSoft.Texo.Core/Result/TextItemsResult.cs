using System.Collections.Generic;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Core.Result
{
    public class TextItemsResult : ICommandResult<IEnumerable<string>>
    {
        public TextItemsResult(IEnumerable<string> content)
        {
            Content = content;
        }

        dynamic ICommandResult.Content => Content;

        public IEnumerable<string> Content { get; }

        public ResultTypeEnum ResultType { get; set; }

        public virtual Task ExecuteResultAsync()
        {
            return Task.CompletedTask;
        }
    }
}
