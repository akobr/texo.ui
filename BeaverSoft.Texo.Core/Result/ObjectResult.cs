using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Core.Result
{
    public class ObjectResult : ICommandResult
    {
        public ObjectResult(object content)
        {
            Content = content;
        }

        public dynamic Content { get; }

        public ResultTypeEnum ResultType { get; set; }

        public Task ExecuteResultAsync()
        {
            return Task.CompletedTask;
        }
    }
}
