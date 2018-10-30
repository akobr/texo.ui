using System.Text.RegularExpressions;

namespace BeaverSoft.Texo.Core.Input
{
    public static class InputRegex
    {
        public const string PATH = "^[^\\<\\>\\\"\\|\\?\\*[:cntrl:]]+$";
        public const string WILDCARD_PATH = "^[^\\<\\>\\\"\\|[:cntrl:]]+$";
        public const string QUERY = "^[A-Za-z_][A-Za-z0-9_-]*$";
        public const string QUERY_OR_OPTION = "^[A-Za-z_-][A-Za-z0-9_-]*$";
        public const string RESERVED = "[\\^\\$\\~\\@\\#\\&\\%\\!\\?\\:\\\"\\'\\\\\\/\\|\\(\\)\\[\\]\\{\\}\\<\\>]+";
        public const string WHITE_SPACES = "[\\s]+";
        public const string VARIABLE_NAME = "[A-Za-z_][A-Za-z0-9_]+";
        public const string VARIABLE = "^" + VARIABLE_NAME + "$";
        public const string INLINE_VARIABLE = "\\$(?<name>" + VARIABLE_NAME + ")\\$";

        public const string GROUP_NAME = "name";

        public static readonly Regex Variable = new Regex(VARIABLE, RegexOptions.Compiled);
        public static readonly Regex InlineVariable = new Regex(INLINE_VARIABLE, RegexOptions.Compiled);
        public static readonly Regex Query = new Regex(QUERY, RegexOptions.Compiled);
        public static readonly Regex QueryOrOption = new Regex(QUERY_OR_OPTION, RegexOptions.Compiled);
        public static readonly Regex Reserved = new Regex(RESERVED, RegexOptions.Compiled);
        public static readonly Regex WhiteSpaces = new Regex(WHITE_SPACES, RegexOptions.Compiled);
        public static readonly Regex Path = new Regex(PATH, RegexOptions.Compiled);
    }
}