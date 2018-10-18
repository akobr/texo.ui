using System.Text.RegularExpressions;

namespace BeaverSoft.Texo.Core.Input
{
    public class InputParseService : IInputParseService
    {
        public IParsedInput Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new EmptyParsedInput();
            }

            Regex tokenExpression = new Regex("\\\"[^\\\"]*\\\"|\\'[^\\']*\\'|[\\S]+", RegexOptions.Compiled);
            MatchCollection matches = tokenExpression.Matches(input);
            string[] tokens = new string[matches.Count];

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                tokens[i] = match.Value;
            }

            return new ParsedInput(input, tokens);
        }
    }
}
