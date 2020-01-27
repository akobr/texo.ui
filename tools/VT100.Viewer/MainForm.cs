using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BeaverSoft.Texo.Core.Console;

namespace VT100.Viewer
{
    public partial class MainForm : Form
    {
        private NativeConsole console;
        private Terminal terminal;

        public MainForm()
        {
            InitializeComponent();
            InitialiseTerminal();
        }

        private void InitialiseTerminal()
        {
            console = new NativeConsole(false);
            terminal = new Terminal();

            //terminal.Start(@"c:\Working\textum.ui\BeaverSoft.Texo.Fallback.PowerShell.Standalone\bin\Debug\BeaverSoft.Texo.Fallback.PowerShell.Standalone.exe", 126, 32);
            terminal.Start(@"c:\Working\textum.ui\tools\Example.Console.App\bin\Debug\Example.Console.App.exe", 126, 32);
            Thread reading = new Thread(CopyConsoleToWindow);
            reading.Start();
        }

        private void CopyConsoleToWindow()
        {
            var buffer = ArrayPool<char>.Shared.Rent(1024);

            try
            {
                using (StreamReader reader = new StreamReader(terminal.Output))
                using (StreamWriter fileWriter = new StreamWriter(File.Open("output.txt", FileMode.Create, FileAccess.Write)) { AutoFlush = true })
                {
                    while (true)
                    {
                        int readed = reader.Read(buffer, 0, buffer.Length);

                        if (readed == 0)
                        {
                            return;
                        }

                        fileWriter.Write(buffer, 0, readed);

                        if (CheckForIllegalCrossThreadCalls)
                        {
                            Invoke(new Action(() => { tbOutput.Text += new string(buffer.Take(readed).ToArray()); }));
                        }
                        else
                        {
                            tbOutput.Text += new string(buffer.Take(readed).ToArray());
                        }
                    }
                }
            }
            catch (Exception exception)
            {

            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            terminal.KillConsole();
        }
    }
}
