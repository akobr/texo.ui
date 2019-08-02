using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Core.Result
{
    public class Result<TContent> : ICommandResult<TContent>
    {
        public Result(TContent content)
        {
            Content = content;
        }

        dynamic ICommandResult.Content => Content;

        public TContent Content { get; }

        public ResultTypeEnum ResultType { get; set; }

        public virtual Task ExecuteResultAsync()
        {
            return Task.CompletedTask;
        }

        public static implicit operator Result<TContent>(TContent obj)
        {
            return new Result<TContent>(obj);
        }
    }
}
