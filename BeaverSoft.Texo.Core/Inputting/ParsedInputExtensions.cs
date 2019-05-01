namespace BeaverSoft.Texo.Core.Inputting
{
    public static class ParsedInputExtensions
    {
        public static bool IsEmpty(this ParsedInput input)
        {
            return input?.Tokens == null || input.Tokens.Count < 1;
        }
    }
}
