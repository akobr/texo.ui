using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Result
{
    public class MarkdownResult : ICommandResult<IItem>
    {
        public MarkdownResult(string markdown)
        {
            Content = new Item.Markdown(markdown);
        }

        dynamic ICommandResult.Content => Content;

        public IItem Content { get; }

        public ResultTypeEnum ResultType { get; set; }

        public Task ExecuteResultAsync()
        {
            return Task.CompletedTask;
        }

        public static implicit operator MarkdownResult(string result)
        {
            return new MarkdownResult(result);
        }
    }
}
