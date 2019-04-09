using BeaverSoft.Texo.Core.Input;

namespace BeaverSoft.Texo.Core.Runtime
{
    public class RealtimeCoordinatorService : IRealtimeCoordinatorService
    {
        private readonly IInputParseService parser;

        public RealtimeCoordinatorService(
            IInputParseService parser)
        {
            this.parser = parser;
        }

        public void Process(string input)
        {
            ParsedInput parsedInput = parser.Parse(input);
        }
    }
}
