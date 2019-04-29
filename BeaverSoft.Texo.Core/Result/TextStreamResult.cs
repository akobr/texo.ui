using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Streaming.Text;

namespace BeaverSoft.Texo.Core.Result
{
    public class TextStreamResult : ICommandResult<IReportableStream>
    {
        public TextStreamResult(ResultTypeEnum resultType, IReportableStream content)
        {
            ResultType = resultType;
            Content = content;
        }

        public TextStreamResult(IReportableStream content)
            : this(ResultTypeEnum.Success, content)
        {
            // no operation
        }

        public ResultTypeEnum ResultType { get; }

        public IReportableStream Content { get; }

        dynamic ICommandResult.Content => Content;
    }
}
