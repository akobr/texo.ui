using System;

namespace PowerShellConsoleHost
{
    class Program
    {
        // https://www.microsoft.com/en-us/download/confirmation.aspx?id=2560
        static void Main(string[] args)
        {
            // Display the welcome message.
            Console.Title = "PowerShell Console Host Sample Application";
            ConsoleColor oldFg = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("    Windows PowerShell Console Host Application Sample");
            Console.WriteLine("    ==================================================");
            Console.WriteLine(string.Empty);
            Console.WriteLine("This is an example of a simple interactive console host uses ");
            Console.WriteLine("the Windows PowerShell engine to interpret commands.");
            Console.WriteLine("Type 'exit' to exit.");
            Console.WriteLine(string.Empty);
            Console.ForegroundColor = oldFg;

            // Create the listener and run it. This method never returns.
            PSListenerConsoleSample listener = new PSListenerConsoleSample();
            listener.Run();
        }
    }
}
