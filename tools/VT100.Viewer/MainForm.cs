using System;
using System.Buffers;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BeaverSoft.Texo.Core.Console;
using BeaverSoft.Texo.Core.Console.Bitmap;
using BeaverSoft.Texo.Core.Console.Decoding;
using BeaverSoft.Texo.Core.Console.Decoding.Ansi;
using BeaverSoft.Texo.Core.Console.Rendering;
using VT100.Viewer.Decoding;

namespace VT100.Viewer
{
    public partial class MainForm : Form
    {
        private const int TERMINAL_WIDTH = 126;
        private const int TERMINAL_HEIGHT = 32;

        //private NativeConsole console;
        private Terminal terminal;
        private bool terminalIsExiting;
        private IDecoder decoder;
        private ConsoleBufferBuilder bufferBuilder;

        public MainForm()
        {
            InitializeComponent();
            Size = new Size(1200, 700);

            InitialiseDecoderClient();
            InitialiseTerminal();
        }

        private void InitialiseDecoderClient()
        {
            var ansi = new AnsiDecoder();
            bufferBuilder = new ConsoleBufferBuilder(TERMINAL_WIDTH, TERMINAL_HEIGHT);
            bufferBuilder.Start();

            ansi.Subscribe(bufferBuilder);
            //ansi.Subscribe(new AnsiDecoderClient(tbOutput, TERMINAL_WIDTH, TERMINAL_HEIGHT));
            decoder = ansi;
        }

        private void InitialiseTerminal()
        {
            //console = new NativeConsole(false);
            terminal = new Terminal();

            //terminal.Start(@"c:\Working\textum.ui\BeaverSoft.Texo.Fallback.PowerShell.Standalone\bin\Debug\BeaverSoft.Texo.Fallback.PowerShell.Standalone.exe", TERMINAL_WIDTH, TERMINAL_HEIGHT);
            //terminal.Start(@"c:\Working\textum.ui\tools\Example.Console.App\bin\Debug\Example.Console.App.exe", TERMINAL_WIDTH, TERMINAL_HEIGHT);
            terminal.Start(@"git status", TERMINAL_WIDTH, TERMINAL_HEIGHT);
            //terminal.Start(@"ping localhost", TERMINAL_WIDTH, TERMINAL_HEIGHT);
            //terminal.Start(@"git show-branch --all", TERMINAL_WIDTH, TERMINAL_HEIGHT);
            //terminal.Start(@"mc", TERMINAL_WIDTH, TERMINAL_HEIGHT);
            //terminal.Start(@"powershell", TERMINAL_WIDTH, TERMINAL_HEIGHT);

            _ = Task.Run(CopyConsoleToWindow);
        }

        private void CopyConsoleToWindow()
        {
            var buffer = ArrayPool<byte>.Shared.Rent(1024);

            try
            {
                using (StreamWriter fileWriter = new StreamWriter(File.Open("output.txt", FileMode.Create, FileAccess.Write)) { AutoFlush = true })
                {
                    while (true)
                    {
                        int readed = terminal.Output.Read(buffer, 0, buffer.Length);

                        if (readed == 0)
                        {
                            return;
                        }

                        fileWriter.Write(Encoding.UTF8.GetString(buffer, 0, readed));
                        OnOutput(buffer, readed);
                    }
                }
            }
            catch (ObjectDisposedException) { /* Pseudo terminal is terminated. */ }
            catch (Exception exception)
            {
                string message = Environment.NewLine + "Error: " + exception.Message;
                Invoke(new Action(() => { tbOutput.Text += message; }));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            terminalIsExiting = true;
            terminal?.Dispose();
            //console?.Dispose();
        }

        private void OnOutput(byte[] buffer, int length)
        {
            if (terminalIsExiting)
            {
                return;
            }

            byte[] validBytes = buffer.Take(length).ToArray();

            if (CheckForIllegalCrossThreadCalls)
            {
                Invoke(new Action(() => { decoder.Input(validBytes); }));
            }
            else
            {
                decoder.Input(validBytes);
            }
        }

        private void HandleSendButtonClick(object sender, EventArgs e)
        {
            ConsoleBuffer buffer = bufferBuilder.Snapshot();
            buffer.ToBitmap().Save("screen.png");
        }
    }
}
