using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Model.Text;

namespace BeaverSoft.Texo.Core.Result
{
    public class ModeledResult : ICommandResult<IElement>
    {
        public ModeledResult(ResultTypeEnum resultType, IElement content)
        {
            ResultType = resultType;
            Content = content;
        }

        public ModeledResult(IElement content)
            : this(ResultTypeEnum.Success, content)
        {
            // no operation
        }

        public ResultTypeEnum ResultType { get; }

        dynamic ICommandResult.Content => Content;

        public IElement Content { get; }
    }
}
