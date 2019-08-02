using System.Dynamic;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Core.Result
{
    public class DynamicResult : ICommandResult
    {
        public DynamicResult()
        {
            Content = new ExpandoObject();
        }

        public dynamic Content { get; }

        public ResultTypeEnum ResultType { get; set; }

        public virtual Task ExecuteResultAsync()
        {
            return Task.CompletedTask;
        }
    }

    public class DynamicResult<TResult> : ICommandResult<TResult>
    {
        public DynamicResult(TResult content)
        {
            Content = content;
        }

        dynamic ICommandResult.Content => Content;

        public TResult Content { get; }

        public ResultTypeEnum ResultType { get; set; }

        public virtual Task ExecuteResultAsync()
        {
            return Task.CompletedTask;
        }
    }
}
