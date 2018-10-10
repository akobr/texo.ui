using System.Text.RegularExpressions;

namespace BeaverSoft.Texo.Core.Input
{
    public static class InputRegex
    {
        public const string GROUP_NAME = "name";
        private const string VARIABLE_NAME = "[A-Za-z_][A-Za-z0-9_]*";

        public static readonly Regex Variable = new Regex($"^{VARIABLE_NAME}$", RegexOptions.Compiled);
        public static readonly Regex InlineVariable = new Regex($"\\$(?<name>{VARIABLE_NAME})\\$", RegexOptions.Compiled);
        public static readonly Regex Query = new Regex("^[A-Za-z_][A-Za-z0-9_-]*$", RegexOptions.Compiled);
        public static readonly Regex QueryOrOption = new Regex("^[A-Za-z_-][A-Za-z0-9_-]*$", RegexOptions.Compiled);
        public static readonly Regex Reserved = new Regex("[\\^\\$\\~\\@\\#\\&\\%\\!\\?\\:\\\"\\'\\\\\\/\\|\\(\\)\\[\\]\\{\\}\\<\\>]+", RegexOptions.Compiled);
        public static readonly Regex WhiteSpaces = new Regex("[\\s]+", RegexOptions.Compiled);
        public static readonly Regex Path = new Regex("^[^\\<\\>\\:\\\"\\|\\?\\*[:cntrl:]]+$", RegexOptions.Compiled);
    }
}