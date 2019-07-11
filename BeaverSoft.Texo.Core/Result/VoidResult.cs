using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Core.Result
{
    public class VoidResult : ICommandResult<string>
    {
        dynamic ICommandResult.Content => Content;

        public string Content => "Command is done";

        public ResultTypeEnum ResultType { get; set; }

        public Task ExecuteResultAsync()
        {
            return Task.CompletedTask;
        }
    }
}
