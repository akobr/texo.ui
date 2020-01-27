using System;
using System.IO;
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
        private FileStream inputStream, outputStream;

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

        public FileStream Input => inputStream;

        public FileStream Output => outputStream;

        public void Start(string shellCommand, short consoleWidth, short consoleHeight)
        {
            input = new Pipe();
            output = new Pipe();

            console = PseudoConsole.Create(input.Read, output.Write, consoleWidth, consoleHeight);
            process = ProcessFactory.Start(shellCommand, PseudoConsole.PseudoConsoleThreadAttribute, console.Handle);

            inputStream = new FileStream(input.Write, FileAccess.Write);
            outputStream = new FileStream(output.Read, FileAccess.Read);
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

            if (disposing)
            {
                inputStream.Dispose();
                outputStream.Dispose();
            }

            process?.Dispose();
            console?.Dispose();
            input?.Dispose();
            output?.Dispose();
        }
    }
}
