using System;

namespace BeaverSoft.Texo.Core.Text
{
    public static class AnsiSgrEscapeCodes
    {
        public const string RESET = "0";
        public const string BOLD = "1";
        public const string ITALIC = "3";
        public const string UNDERLINE = "4";
        public const string CROSSED_OUT = "9";

        public const string FOREGROUND_BLACK = "30";
        public const string FOREGROUND_GRAY = "30;1";
        public const string FOREGROUND_RED = "31";
        public const string FOREGROUND_GREEN = "32";
        public const string FOREGROUND_YELLOW = "33";
        public const string FOREGROUND_BLUE = "34";
        public const string FOREGROUND_MAGENTA = "35";
        public const string FOREGROUND_CYAN = "36";
        public const string FOREGROUND_WHITE = "37";

        public const string BACKGROUND_BLACK = "40";
        public const string BACKGROUND_RED = "41";
        public const string BACKGROUND_GREEN = "42";
        public const string BACKGROUND_YELLOW = "43";
        public const string BACKGROUND_BLUE = "44";
        public const string BACKGROUND_MAGENTA = "45";
        public const string BACKGROUND_CYAN = "46";
        public const string BACKGROUND_WHITE = "47";

        public const string LINK_START = "999";
        public const char LINK_SEPARATOR = '|';
        public const string LINK_END = "998";

        public static string GetForegroundColorCode(byte red, byte green, byte blue)
        {
            return $"38;2;{red};{green};{blue}";
        }

        public static string GetBackgroundColorCode(byte red, byte green, byte blue)
        {
            return $"48;2;{red};{green};{blue}";
        }

        public static string GetForegroundColorCode(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                    return FOREGROUND_BLACK;

                case ConsoleColor.Gray:
                    return FOREGROUND_GRAY;

                case ConsoleColor.Red:
                case ConsoleColor.DarkRed:
                    return FOREGROUND_RED;

                case ConsoleColor.Green:
                case ConsoleColor.DarkGreen:
                    return FOREGROUND_GREEN;

                case ConsoleColor.Yellow:
                case ConsoleColor.DarkYellow:
                    return FOREGROUND_YELLOW;

                case ConsoleColor.Blue:
                case ConsoleColor.DarkBlue:
                    return FOREGROUND_BLUE;

                case ConsoleColor.Magenta:
                case ConsoleColor.DarkMagenta:
                    return FOREGROUND_MAGENTA;

                case ConsoleColor.Cyan:
                case ConsoleColor.DarkCyan:
                    return FOREGROUND_CYAN;

                case ConsoleColor.White:
                    return FOREGROUND_WHITE;

                default:
                    return null;
            }
        }

        public static string GetBackgroundColorCode(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                    return BACKGROUND_BLACK;

                case ConsoleColor.Red:
                case ConsoleColor.DarkRed:
                    return BACKGROUND_RED;

                case ConsoleColor.Green:
                case ConsoleColor.DarkGreen:
                    return BACKGROUND_GREEN;

                case ConsoleColor.Yellow:
                case ConsoleColor.DarkYellow:
                    return BACKGROUND_YELLOW;

                case ConsoleColor.Blue:
                case ConsoleColor.DarkBlue:
                    return BACKGROUND_BLUE;

                case ConsoleColor.Magenta:
                case ConsoleColor.DarkMagenta:
                    return BACKGROUND_MAGENTA;

                case ConsoleColor.Cyan:
                case ConsoleColor.DarkCyan:
                    return BACKGROUND_CYAN;

                case ConsoleColor.White:
                    return BACKGROUND_WHITE;

                default:
                    return null;
            }
        }
    }
}
