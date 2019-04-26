using System;

namespace Commands.Clipboard
{
    public interface IClipboardMonitor
    {
        event EventHandler<EventArgs> ClipboardChanged;

        bool InvokeRequired { get; }

        object Invoke(Delegate action);
    }
}