using System;

using SysConsole = System.Console;

namespace BeaverSoft.Texo.View.Console
{
    public static class TexoConsole
    {
        private const char SPACE = ' ';

        public static void WritePrompt(string prompt)
        {
            SysConsole.Write(SPACE);
            WriteWithColor($"{prompt}>", ConsoleColor.Gray, ConsoleColor.DarkGray);
            SysConsole.Write(SPACE);
        }

        public static void WriteWithColor(string text, ConsoleColor foreground, ConsoleColor? background = null)
        {
            SysConsole.ForegroundColor = foreground;

            if (background != null)
            {
                SysConsole.BackgroundColor = background.Value;
            }

            SysConsole.Write(text);
            SysConsole.ResetColor();
        }
    }
}
