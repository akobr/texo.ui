using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Commands.Clipboard
{
    [DefaultEvent("ClipboardChanged")]
    public class ClipboardMonitor : Control
    {
        IntPtr nextClipboardViewer;

        public ClipboardMonitor()
        {
            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)Handle);
        }

        public event EventHandler<ClipboardChangedEventArgs> ClipboardChanged;

        protected override void Dispose(bool disposing)
        {
            ChangeClipboardChain(Handle, nextClipboardViewer);
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
                        nextClipboardViewer = m.LParam;
                    else
                        SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        void OnClipboardChanged()
        {
            IDataObject iData = System.Windows.Forms.Clipboard.GetDataObject();
            ClipboardChanged?.Invoke(this, new ClipboardChangedEventArgs(iData));
        }
    }
}
