using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Streaming.Text;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Result
{
    public class TextStreamResult : ICommandResult<ITextStream>
    {
        public TextStreamResult(ResultTypeEnum resultType, ITextStream content, Task outerTask)
        {
            ResultType = resultType;
            Content = content;
            OuterTask = outerTask;
        }

        public TextStreamResult(ITextStream content, Task outputTask)
            : this(ResultTypeEnum.Success, content, outputTask)
        {
            // no operation
        }

        public ResultTypeEnum ResultType { get; }

        public Task OuterTask { get; }

        public ITextStream Content { get; }

        dynamic ICommandResult.Content => Content;
    }
}
