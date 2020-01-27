using System;
using System.IO;
using System.Text;
using System.Threading;
using BeaverSoft.Texo.Core.Console.Interop;
using Microsoft.Win32.SafeHandles;

namespace BeaverSoft.Texo.Core.Console
{
    public class Terminal : IDisposable
    {
        private Pipe input;
        private Pipe output;
        private PseudoConsole console;
        private Process process;
        private bool disposed;

        public Terminal()
        {
            ConPtyFeature.ThrowIfVirtualTerminalIsNotEnabled();

            if (ConsoleApi.GetConsoleWindow() != IntPtr.Zero)
            {
                ConPtyFeature.TryEnableVirtualTerminalConsoleSequenceProcessing();
            }
        }

        ~Terminal()
        {
            Dispose(false);
        }

        public FileStream Input { get; private set; }

        public FileStream Output { get; private set; }

        public void Start(string shellCommand, short consoleWidth, short consoleHeight)
        {
            input = new Pipe();
            output = new Pipe();

            console = PseudoConsole.Create(input.Read, output.Write, consoleWidth, consoleHeight);
            process = ProcessFactory.Start(shellCommand, PseudoConsole.PseudoConsoleThreadAttribute, console.Handle);

            Input = new FileStream(input.Write, FileAccess.Write);
            Output = new FileStream(output.Read, FileAccess.Read);
        }

        public void KillConsole()
        {
            console?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public WaitHandle BuildWaitHandler()
        {
            return new AutoResetEvent(false)
            {
                SafeWaitHandle = new SafeWaitHandle(process.ProcessInfo.hProcess, ownsHandle: false)
            };
        }

        public void WaitToExit()
        {
            BuildWaitHandler().WaitOne(Timeout.Infinite);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            process?.Dispose();
            console?.Dispose();

            if (disposing)
            {
                Input?.Dispose();
                //FakeEndOfFile(output?.Write);
                Output?.Dispose();
            }

            input?.Dispose();
            output?.Dispose();
        }

        private void FakeEndOfFile(SafeFileHandle fileHandler)
        {
            if (fileHandler == null || fileHandler.IsInvalid || fileHandler.IsClosed)
            {
                return;
            }

            using (FileStream stream = new FileStream(new SafeFileHandle(fileHandler.DangerousGetHandle(), false), FileAccess.Write))
            {
                byte[] endBytes = Encoding.UTF8.GetBytes("\n" + (char)26);
                stream.Write(endBytes, 0, endBytes.Length);
            }
        }
    }
}
