namespace BeaverSoft.Texo.Core.Inputting
{
    public static class InputConstants
    {
        public const char PRIMARY_STRING_CHARACTER = '\"';
        public const char SECONDARY_STRING_CHARACTER = '\'';

        public static char[] RESERVED_CHARACTERS =
        {
            '^', '$' ,'~', '@', '#', '&', '%', '!', '?',
            PRIMARY_STRING_CHARACTER, SECONDARY_STRING_CHARACTER,
            '\\', '/', '|', '(', ')', '[', ']', '{', '}', '<', '>'
        };

        public const char PARAMETER_INPUT_PREFIX = '-';
        public const string END_OF_PARAMETER_LIST = "--";
    }
}
