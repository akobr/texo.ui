namespace BeaverSoft.Texo.Core.Commands
{
    public interface ICommandResult
    {
        ResultTypeEnum ResultType { get; }

        dynamic Content { get; }
    }

    public interface ICommandResult<out TResult> : ICommandResult
    {
        new TResult Content { get; }
    }
}