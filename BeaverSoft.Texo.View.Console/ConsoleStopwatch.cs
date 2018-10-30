using System;
using System.Diagnostics;

namespace BeaverSoft.Texo.View.Console
{
    public class ConsoleStopwatch : IDisposable
    {
        private readonly Stopwatch stopwatch;

        public ConsoleStopwatch()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public void Dispose()
        {
            stopwatch.Stop();
            TexoConsole.WriteWithColor($"The operation takes {stopwatch.Elapsed:g}.", ConsoleColor.DarkYellow);
            System.Console.WriteLine();
        }
    }
}
