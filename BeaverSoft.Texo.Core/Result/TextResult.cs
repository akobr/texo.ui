using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Core.Result
{
    public class TextResult : ICommandResult<string>
    {
        public TextResult(ResultTypeEnum resultType, string content)
        {
            ResultType = resultType;
            Content = content;
        }

        public TextResult(string content)
            : this(ResultTypeEnum.Success, content)
        {
            // no operation
        }

        public ResultTypeEnum ResultType { get; }

        dynamic ICommandResult.Content => Content;

        public string Content { get; }

        public static implicit operator TextResult(string result)
        {
            return new TextResult(result);
        }
    }
}
