using System.Dynamic;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Core.Result
{
    public class DynamicResult : ICommandResult
    {
        public DynamicResult(ResultTypeEnum resultType)
        {
            ResultType = resultType;
            Content = new ExpandoObject();
        }

        public DynamicResult()
            : this(ResultTypeEnum.Success)
        {
            // no operation
        }

        public ResultTypeEnum ResultType { get; }

        public dynamic Content { get; }
    }

    public class DynamicResult<TResult> : ICommandResult<TResult>
    {
        public DynamicResult(ResultTypeEnum resultType, TResult content)
        {
            ResultType = resultType;
            Content = content;
        }

        public DynamicResult(TResult content)
        : this(ResultTypeEnum.Success, content)
        {
            // no operation
        }

        public ResultTypeEnum ResultType { get; }

        public TResult Content { get; }

        dynamic ICommandResult.Content => Content;
    }
}
