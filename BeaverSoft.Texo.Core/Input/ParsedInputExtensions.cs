namespace BeaverSoft.Texo.Core.Input
{
    public static class ParsedInputExtensions
    {
        public static bool IsEmpty(this ParsedInput input)
        {
            return input?.Tokens == null || input.Tokens.Count < 1;
        }
    }
}
