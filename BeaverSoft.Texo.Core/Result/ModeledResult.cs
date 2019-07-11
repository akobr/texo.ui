using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Model.Text;

namespace BeaverSoft.Texo.Core.Result
{
    public class ModeledResult : ICommandResult<IElement>
    {
        public ModeledResult(IElement content)
        {
            Content = content;
        }

        dynamic ICommandResult.Content => Content;

        public IElement Content { get; }

        public ResultTypeEnum ResultType { get; set; }

        public Task ExecuteResultAsync()
        {
            return Task.CompletedTask;
        }
    }
}
