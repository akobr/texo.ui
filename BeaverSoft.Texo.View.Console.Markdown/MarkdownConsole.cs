using System;

using SysConsole = System.Console;

namespace BeaverSoft.Texo.View.Console.Markdown
{
    public static class MarkdownConsole
    {
        public static void WriteHeader(string text, int level)
        {
            TexoConsole.WriteWithColor(text, GetHeaderColor(level));
        }

        public static void WriteItalic(string text)
        {
            TexoConsole.WriteWithColor(text, ConsoleColor.DarkGray);
        }

        public static void WriteBold(string text)
        {
            TexoConsole.WriteWithColor(text, ConsoleColor.DarkMagenta);
        }

        public static void WriteCode(string code)
        {
            TexoConsole.WriteWithColor(code, ConsoleColor.DarkGreen, ConsoleColor.White);
        }

        public static void WriteQuote(string quote)
        {
            TexoConsole.WriteWithColor(quote, ConsoleColor.Black, ConsoleColor.White);
        }

        public static void WriteLink(string url)
        {
            TexoConsole.WriteWithColor(url, ConsoleColor.DarkBlue);
        }

        public static void WriteLink(string url, string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                TexoConsole.WriteWithColor(title, ConsoleColor.DarkBlue);
                SysConsole.Write(" / ");
            }

            TexoConsole.WriteWithColor(url, ConsoleColor.DarkBlue);
        }

        public static void WriteListItemBullet(string bullet, int intentLevel)
        {
            if (intentLevel <= 1)
            {
                SysConsole.Write(' ');
            }
            else
            {
                SysConsole.Write(new string(' ', intentLevel * 2 - 1));
            }
            
            TexoConsole.WriteWithColor(bullet, ConsoleColor.Yellow, ConsoleColor.DarkGreen);
            SysConsole.Write(' ');
        }

        public static void WriteHtml(string html)
        {
            TexoConsole.WriteWithColor(html, ConsoleColor.DarkCyan, ConsoleColor.White);
        }

        public static void WriteHorizontalBreak()
        {
            SysConsole.WriteLine();
            TexoConsole.WriteWithColor(new string('-', 32), ConsoleColor.DarkGray, ConsoleColor.DarkGray);
            SysConsole.WriteLine();
        }

        private static ConsoleColor GetHeaderColor(int level)
        {
            if (level <= 1)
            {
                return ConsoleColor.Green;
            }

            if (level >= 3)
            {
                return ConsoleColor.Blue;
            }

            return ConsoleColor.Cyan;
        }
    }
}