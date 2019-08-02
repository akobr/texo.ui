using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Streaming;

namespace BeaverSoft.Texo.Core.Result
{
    public class TextStreamResult : ICommandResult<IReportableStream>
    {
        public TextStreamResult(IReportableStream content)
        {
            Content = content;
        }

        dynamic ICommandResult.Content => Content;

        public IReportableStream Content { get; }

        public ResultTypeEnum ResultType { get; set; }

        public Task ExecuteResultAsync()
        {
            return Task.CompletedTask;
        }
    }
}
