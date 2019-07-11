using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Commands
{
    public interface ICommandResult
    {
        dynamic Content { get; }

        ResultTypeEnum ResultType { get; }

        Task ExecuteResultAsync();
    }

    public interface ICommandResult<out TResult> : ICommandResult
    {
        new TResult Content { get; }
    }
}