using System;

namespace Commands.Clipboard
{
    public interface IClipboardMonitor
    {
        event EventHandler<EventArgs> ClipboardChanged;
    }
}