using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Core.Result
{
    public class ErrorTextResult : TextResult
    {
        public ErrorTextResult(string content)
            : base(content)
        {
            ResultType = ResultTypeEnum.Failed;
        }
    }
}
