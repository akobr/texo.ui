using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using StrongBeaver.Core.Services;

namespace Commands.Clipboard
{
    public class ClipboardMonitorForm : Form, IClipboardMonitor
    {
        private readonly IServiceMessageBus messageBus;
        private IntPtr nextClipboardViewer;

        public event EventHandler<EventArgs> ClipboardChanged;

        public ClipboardMonitorForm(IServiceMessageBus messageBus)
        {
            this.messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));

            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            Opacity = 0.0;
            SetVisibleCore(false);
        }

        public void Initialise(IWin32Window window)
        {
            Show(window);
            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)Handle);
            messageBus.Send(new ClipboardMonitorReadyMessage(this));
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                ChangeClipboardChain(Handle, nextClipboardViewer);
            }
            catch (Win32Exception)
            {
                // swallows error with non-existing window handler
            }
        }

        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        protected override void WndProc(ref Message m)
        {
            // defined in winuser.h
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    OnClipboardChanged();
                    SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                case WM_CHANGECBCHAIN:
                    if (m.WParam == nextClipboardViewer)
                    {
                        nextClipboardViewer = m.LParam;
                    }
                    else
                    {
                        SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    }
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        void OnClipboardChanged()
        {
            ClipboardChanged?.Invoke(this, new EventArgs());
        }
    }
}
