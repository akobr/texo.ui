using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Core.Result
{
    public class ErrorTextResult : TextResult
    {
        public ErrorTextResult(string content)
            : base(ResultTypeEnum.Failed, content)
        {
            // no operation
        }
    }
}
