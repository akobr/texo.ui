namespace BeaverSoft.Texo.Core.Input
{
    public static class ParsedInputExtensions
    {
        public static bool IsEmpty(this IParsedInput input)
        {
            return input?.Tokens == null || input.Tokens.Count < 1;
        }
    }
}
