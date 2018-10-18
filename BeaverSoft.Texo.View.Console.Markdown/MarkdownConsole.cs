using System;

using SysConsole = System.Console;

namespace BeaverSoft.Texo.View.Console.Markdown
{
    public static class MarkdownConsole
    {
        public static void WriteHeader(string text, int level)
        {
            TexoConsole.WriteWithColor(text, GetHeaderColor(level));
            SysConsole.WriteLine();
        }

        public static void WriteItalic(string text)
        {
            TexoConsole.WriteWithColor(text, ConsoleColor.DarkMagenta);
        }

        public static void WriteBold(string text)
        {
            TexoConsole.WriteWithColor(text, ConsoleColor.Magenta);
        }

        public static void WriteCode(string code)
        {
            TexoConsole.WriteWithColor(code, ConsoleColor.Yellow, ConsoleColor.DarkGray);
        }

        public static void WriteQuote(string quote)
        {
            TexoConsole.WriteWithColor(quote, ConsoleColor.DarkYellow, ConsoleColor.DarkGray);
            SysConsole.WriteLine();
        }

        public static void WriteLink(string url)
        {
            TexoConsole.WriteWithColor(url, ConsoleColor.Blue, ConsoleColor.DarkGray);
        }

        public static void WriteLink(string url, string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                TexoConsole.WriteWithColor(title, ConsoleColor.DarkBlue, ConsoleColor.DarkGray);
                SysConsole.Write(" / ");
            }

            TexoConsole.WriteWithColor(url, ConsoleColor.Blue, ConsoleColor.DarkGray);
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
            TexoConsole.WriteWithColor(html, ConsoleColor.White, ConsoleColor.DarkCyan);
        }

        public static void WriteHorizontalBreak()
        {
            SysConsole.WriteLine();
            TexoConsole.WriteWithColor("---", ConsoleColor.DarkGray, ConsoleColor.DarkGray);
            SysConsole.WriteLine();
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