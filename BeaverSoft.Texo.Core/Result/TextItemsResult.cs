using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Core.Result
{
    public class TextItemsResult : ICommandResult<IEnumerable<string>>
    {
        public TextItemsResult(ResultTypeEnum resultType, IEnumerable<string> content)
        {
            ResultType = resultType;
            Content = content;
        }

        public TextItemsResult(IEnumerable<string> content)
            : this(ResultTypeEnum.Success, content)
        {
            // no operation
        }

        public TextItemsResult(params string[] content)
            : this(ResultTypeEnum.Success, ImmutableList.Create<string>(content))
        {
            // no operation
        }

        public ResultTypeEnum ResultType { get; }

        dynamic ICommandResult.Content => Content;

        public IEnumerable<string> Content { get; }

    }
}
