using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Core.Result
{
    public class MarkdownResult : TextResult
    {
        public MarkdownResult(ResultTypeEnum resultType, string markdown)
            : base(resultType, markdown)
        {
            // no operation
        }

        public MarkdownResult(string markdown)
            : base(ResultTypeEnum.Success, markdown)
        {
            // no operation
        }
    }
}
