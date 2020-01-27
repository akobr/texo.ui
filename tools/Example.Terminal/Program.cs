using System;
using System.IO;
using System.Threading.Tasks;

namespace Example.Terminal
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var terminal = new BeaverSoft.Texo.Core.Console.Terminal())
                {
                    //terminal.Start(@"c:\Working\textum.ui\BeaverSoft.Texo.Fallback.PowerShell.Standalone\bin\Debug\BeaverSoft.Texo.Fallback.PowerShell.Standalone.exe", 126, 32);
                    //terminal.Start(@"c:\Working\textum.ui\tools\Example.Console.App\bin\Debug\Example.Console.App.exe", 126, 32);
                    terminal.Start(@"ping localhost", 126, 32);
                    _ = Task.Run(() => CopyPipeToOutput(terminal.Output));
                    terminal.WaitToExit();
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                throw;
            }

            Console.ReadKey();
        }

        private static void CopyPipeToOutput(Stream output)
        {
            try
            {
                char[] buffer = new char[1024];

                using (StreamReader reader = new StreamReader(output))
                using (var terminalOutput = Console.OpenStandardOutput())
                using (StreamWriter writer = new StreamWriter(terminalOutput) { AutoFlush = true })
                using (StreamWriter fileWriter = new StreamWriter(File.Open("output.txt", FileMode.Create, FileAccess.Write)) { AutoFlush = true })
                {
                    while (true)
                    {
                        int readed = reader.Read(buffer, 0, buffer.Length);

                        if (readed == 0)
                        {
                            return;
                        }

                        writer.Write(buffer, 0, readed);
                        fileWriter.Write(buffer, 0, readed);
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Terminal has been terminated.");
            }
        }
    }
}
