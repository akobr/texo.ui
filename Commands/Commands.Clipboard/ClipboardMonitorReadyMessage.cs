using StrongBeaver.Core.Services;

namespace Commands.Clipboard
{
    public class ClipboardMonitorReadyMessage : ServiceMessage
    {
        public ClipboardMonitorReadyMessage(IClipboardMonitor monitor)
        {
            Monitor = monitor;
        }

        public IClipboardMonitor Monitor { get; }
    }
}
