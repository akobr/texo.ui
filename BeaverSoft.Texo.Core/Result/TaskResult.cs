using System;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Extensions;

namespace BeaverSoft.Texo.Core.Result
{
    public class TaskResult : ICommandResult<string>
    {
        private readonly Task task;
        private ResultTypeEnum resultType;

        public TaskResult(Task task)
        {
            this.task = task ?? throw new ArgumentNullException(nameof(task));
        }

        dynamic ICommandResult.Content => Content;

        public string Content => task.IsFinished()
            ? "Command is running"
            : "Command is done";

        public ResultTypeEnum ResultType
        {
            get
            {
                return task.IsFinished()
                    ? resultType
                    : ResultTypeEnum.InProgress;
            }

            set => resultType = value;
        }

        public async Task ExecuteResultAsync()
        {
            await task;
        }
    }

    public class TaskResult<TContent> : ICommandResult<TContent>
    {
        private readonly Task<TContent> task;
        private ResultTypeEnum resultType;

        public TaskResult(Task<TContent> task)
        {
            this.task = task;
        }

        dynamic ICommandResult.Content => Content;

        public TContent Content => task.Result;

        public ResultTypeEnum ResultType
        {
            get
            {
                return task.IsFinished()
                    ? resultType
                    : ResultTypeEnum.InProgress;
            }

            set => resultType = value;
        }

        public async virtual Task ExecuteResultAsync()
        {
            await task;
        }

        public static implicit operator TaskResult<TContent>(Task<TContent> task)
        {
            return new TaskResult<TContent>(task);
        }
    }
}
