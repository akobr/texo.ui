using System;
using System.IO;
using System.Threading;
using BeaverSoft.Texo.Core.Console.Interop;
using BeaverSoft.Texo.Core.Console.Interop.Definitions;

namespace BeaverSoft.Texo.Core.Console
{
    public class NativeConsole : IDisposable
    {
        private IntPtr handle;
        //private System.Diagnostics.Process process;
        private bool isDisposed;
        private Pipe stdOut, stdErr, stdIn;
        private FileStream stdOutStream, stdErrStream, stdInStream;

        public NativeConsole(bool hidden = true)
        {
            Initialise(hidden);
        }

        ~NativeConsole()
        {
            Dispose(false);
        }

        public FileStream Output => stdOutStream;

        public FileStream Error => stdErrStream;

        public FileStream Input => stdInStream;

        public static void SendCtrlEvent(CtrlEvent ctrlEvent)
        {
            ConsoleApi.GenerateConsoleCtrlEvent(ctrlEvent, 0);
        }

        public static void RegisterOnCloseAction(Action action)
        {
            RegisterCtrlEventFunction((ctrlEvent) =>
            {
                if (ctrlEvent == CtrlEvent.CtrlClose)
                {
                    action();
                }

                return false;
            });
        }

        public static void RegisterCtrlEventFunction(CtrlEventDelegate function)
        {
            ConsoleApi.SetConsoleCtrlHandler(function, true);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;

            if (disposing)
            {
                stdInStream.Dispose();
                stdOutStream.Dispose();
                stdErrStream.Dispose();
            }

            ConsoleApi.FreeConsole();
            ReleaseUnmanagedResources();
        }

        private void ReleaseUnmanagedResources()
        {
            stdIn.Dispose();
            stdOut.Dispose();
            stdErr.Dispose();
        }

        private void Initialise(bool hidden)
        {
            if (!ConsoleApi.AllocConsole())
            {
                throw InteropException.CreateWithInnerHResultException("Could not allocate console. You may need to FreeConsole first.");
            }

            handle = ConsoleApi.GetConsoleWindow();

            if (handle != IntPtr.Zero)
            {
                ConsoleApi.ShowWindow(handle, hidden ? ShowState.SwHide : ShowState.SwShowDefault);
            }

            RegisterOnCloseAction(ReleaseUnmanagedResources);
            //process = System.Diagnostics.Process.GetCurrentProcess();

            CreateStdOutPipe();
            CreateStdErrPipe();
            CreateStdInPipe();
        }

        private void CreateStdOutPipe()
        {
            stdOut = new Pipe();
            if (!ConsoleApi.SetStdHandle(StdHandle.OutputHandle, stdOut.Write.DangerousGetHandle()))
            {
                throw InteropException.CreateWithInnerHResultException("Could not redirect STDOUT.");
            }
            //stdOut.MakeReadNoninheritable(process.Handle);
            stdOutStream = new FileStream(stdOut.Read, FileAccess.Read);
        }

        private void CreateStdErrPipe()
        {
            stdErr = new Pipe();
            if (!ConsoleApi.SetStdHandle(StdHandle.ErrorHandle, stdErr.Write.DangerousGetHandle()))
            {
                throw InteropException.CreateWithInnerHResultException("Could not redirect STDERR.");
            }
            //stdErr.MakeReadNoninheritable(process.Handle);
            stdErrStream = new FileStream(stdErr.Read, FileAccess.Read);
        }

        private void CreateStdInPipe()
        {
            stdIn = new Pipe();
            if (!ConsoleApi.SetStdHandle(StdHandle.InputHandle, stdIn.Read.DangerousGetHandle()))
            {
                throw InteropException.CreateWithInnerHResultException("Could not redirect STDIN.");
            }
            //stdIn.MakeWriteNoninheritable(process.Handle);
            stdInStream = new FileStream(stdIn.Write, FileAccess.Write);
        }
    }
}
