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
using BeaverSoft.Texo.Core.Console.Interop;
using BeaverSoft.Texo.Core.Console.Rendering;
using VT100.Viewer.Decoding;

namespace VT100.Viewer
{
    public partial class MainForm : Form
    {
        private const int TERMINAL_WIDTH = 128;
        private const int TERMINAL_HEIGHT = 32;

        //private NativeConsole console;
        private Terminal terminal;
        private bool terminalIsExiting;
        private IDecoder decoder;
        private ConsoleBufferBuilder bufferBuilder;
        private System.Windows.Forms.Keys lastKeyData;
        private Bitmap bufferImage;

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
            ansi.Output += OnDecoderInput;
            decoder = ansi;
        }

        private void InitialiseTerminal()
        {
            //console = new NativeConsole(false);
            terminal = new Terminal();

            //terminal.Start(@"c:\Working\textum.ui\BeaverSoft.Texo.Fallback.PowerShell.Standalone\bin\Debug\BeaverSoft.Texo.Fallback.PowerShell.Standalone.exe", TERMINAL_WIDTH, TERMINAL_HEIGHT);
            //terminal.Start(@"c:\Working\textum.ui\tools\Example.Console.App\bin\Debug\Example.Console.App.exe", TERMINAL_WIDTH, TERMINAL_HEIGHT);
            //terminal.Start(@"git status", TERMINAL_WIDTH, TERMINAL_HEIGHT);
            //terminal.Start(@"ping localhost", TERMINAL_WIDTH, TERMINAL_HEIGHT);
            //terminal.Start(@"git show-branch --all", TERMINAL_WIDTH, TERMINAL_HEIGHT);
            //terminal.Start(@"mc", TERMINAL_WIDTH, TERMINAL_HEIGHT);
            terminal.Start(@"powershell", TERMINAL_WIDTH, TERMINAL_HEIGHT);
            //terminal.Start(@"cmd", TERMINAL_WIDTH, TERMINAL_HEIGHT);
            //terminal.Start("\"C:\\Program Files\\Git\\git-bash.exe\" --cd-to-home", TERMINAL_WIDTH, TERMINAL_HEIGHT);
            _ = Task.Run(CopyConsoleToWindow);
        }

        private void CopyConsoleToWindow()
        {
            var buffer = ArrayPool<byte>.Shared.Rent(1024);

            try
            {
                File.Delete("output.txt");

                while (true)
                {
                    int readed = terminal.Output.Read(buffer, 0, buffer.Length);

                    if (readed == 0)
                    {
                        return;
                    }

                    using (StreamWriter fileWriter = new StreamWriter(File.Open("output.txt", FileMode.Append, FileAccess.Write)) { AutoFlush = true })
                    {
                        fileWriter.Write(Encoding.UTF8.GetString(buffer, 0, readed));
                    }

                    OnOutput(buffer, readed);
                }
            }
            catch (ObjectDisposedException) { /* Pseudo terminal is terminated. */ }
            catch (Exception exception)
            {
                string message = Environment.NewLine + "Error: " + exception.Message;
                Invoke(new Action(() => { MessageBox.Show(message); }));
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

            //if (CheckForIllegalCrossThreadCalls)
            //{
            //    Invoke(new Action(() => { decoder.Input(validBytes); }));
            //}
            //else
            //{
                decoder.Input(validBytes);
            //}

            try
            {
                RenderBuffer();
            }
            catch (Exception) { }
        }

        private void RenderBuffer()
        {
            if (bufferImage == null)
            {
                return;
            }

            ConsoleBuffer buffer = bufferBuilder.Snapshot(ConsoleBufferType.Screen);
            buffer.RenderScreenChangesToExistingBitmap(bufferImage);

            Invoke(new Action(() => {
                pbBuffer.Image = bufferImage;
            }));
        }

        private void OnDecoderInput(IDecoder decoder, byte[] data)
        {
            terminal.Input.Write(data, 0, data.Length);
            terminal.Input.Flush();
        }

        private void HandleSendButtonClick(object sender, EventArgs e)
        {
            ConsoleBuffer buffer = bufferBuilder.Snapshot(ConsoleBufferType.AllChanges);
            buffer.ToBitmap().Save("screen.png");
            bufferImage = buffer.ToScreenBitmap();
            pbBuffer.Image = bufferImage;
        }

        private async void HandleRawInputTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            const char SPACE = ' ';
            e.SuppressKeyPress = true;

            if (e.KeyData == lastKeyData)
            {
                return;
            }

            if (tbRawInput.Text.Split(SPACE).Length > 10)
            {
                int separatorIndex = tbRawInput.Text.IndexOf(SPACE);
                tbRawInput.Text = tbRawInput.Text.Substring(separatorIndex + 1);
            }

            string keyString = null;

            if (!decoder.KeyPressed(
                (BeaverSoft.Texo.Core.Console.Keys)(int)e.Modifiers,
                (BeaverSoft.Texo.Core.Console.Keys)(int)e.KeyCode))
            {
                keyString = Keyboard.GetCharsFromKeys((uint)e.KeyCode, e.Shift);
                byte[] bytes = Encoding.UTF8.GetBytes(keyString);

                if (bytes.Length < 1)
                {
                    return;
                }

                terminal.Input.Write(bytes, 0, bytes.Length);
                terminal.Input.Flush();
            }

            KeysConverter kc = new KeysConverter();
            string inputDescription = SPACE + kc.ConvertToString(e.KeyData);
            if (!string.IsNullOrWhiteSpace(keyString)) inputDescription += $"({keyString})";

            lastKeyData = e.KeyData;
            tbRawInput.Text += inputDescription;
            //await Task.Delay(250);
            //RenderBuffer();
        }

        private void HandleRawInputTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            lastKeyData = System.Windows.Forms.Keys.None;
        }

        private void HandleRawInputTextBoxEnter(object sender, EventArgs e)
        {
            tbRawInput.BackColor = Color.LightGreen;
        }

        private void HandleRawInputTextBoxLeave(object sender, EventArgs e)
        {
            tbRawInput.BackColor = DefaultBackColor;
        }
    }
}
