using System;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone
{
    public static class NativeConsole
    {
        public static void WriteControlLine(string message)
        {
            Console.WriteLine($"$${message}$$");
        }
    }
}
