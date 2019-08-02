using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Core.Result
{
    public class TextResult : ICommandResult<string>
    {
        public TextResult(string content)
        {
            Content = content;
        }

        dynamic ICommandResult.Content => Content;

        public string Content { get; }

        public ResultTypeEnum ResultType { get; set; }

        public static implicit operator TextResult(string result)
        {
            return new TextResult(result);
        }

        public virtual Task ExecuteResultAsync()
        {
            return Task.CompletedTask;
        }
    }
}
