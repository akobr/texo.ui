using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Core.Result
{
    public class MarkdownItemsResult : TextItemsResult
    {
        public MarkdownItemsResult(ResultTypeEnum resultType, IEnumerable<string> content)
            : base(resultType, content)
        {
            // no operation
        }

        public MarkdownItemsResult(IEnumerable<string> content)
            : base(ResultTypeEnum.Success, content)
        {
            // no operation
        }

        public MarkdownItemsResult(params string[] content)
            : base(ResultTypeEnum.Success, ImmutableList.Create<string>(content))
        {
            // no operation
        }
    }
}
