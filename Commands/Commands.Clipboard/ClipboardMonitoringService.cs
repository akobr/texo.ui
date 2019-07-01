using StrongBeaver.Core.Messaging;
using StrongBeaver.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Commands.Clipboard
{
    public class ClipboardMonitoringService : IMessageBusService<ClipboardMonitorReadyMessage>, IDisposable
    {
        private const int HISTORY_SIZE = 42; // Answer to the Ultimate Question of Life, the Universe, and Everything.

        private IClipboardMonitor monitor;
        private HashSet<IClipboardItem> historySet;
        private List<IClipboardItem> history;

        public ClipboardMonitoringService()
        {
            Configuration = new ClipboardConfiguration();
            historySet = new HashSet<IClipboardItem>();
            history = new List<IClipboardItem>();
        }

        public int HistoryCount => history.Count;

        public ClipboardConfiguration Configuration { get; }

        public IClipboardItem SetClipboardFromHistory(int index)
        {
            index = history.Count - 1 - index;

            if (index < 0 || index >= history.Count)
            {
                return null;
            }

            if (monitor.InvokeRequired)
            {
                return (IClipboardItem)monitor.Invoke((Func<IClipboardItem>)(() => SetClipboardFromHistory(index)));
            }

            IClipboardItem item = history[index];
            System.Windows.Forms.Clipboard.Clear();
            System.Windows.Forms.Clipboard.SetText(item.Content);
            return item;
        }

        public IClipboardItem GetHistoryItem(int index)
        {
            index = history.Count - 1 - index;

            if (index < 0 || index >= history.Count)
            {
                return null;
            }

            return history[index];
        }

        public IEnumerable<IClipboardItem> GetHistory()
        {
            return ((IEnumerable<IClipboardItem>)history).Reverse();
        }

        public void ClearHistory()
        {
            historySet.Clear();
            history.Clear();
        }

        public void Dispose()
        {
            DisposeMonitor();
        }

        void IMessageBusRecipient<ClipboardMonitorReadyMessage>.ProcessMessage(ClipboardMonitorReadyMessage message)
        {
            monitor = message.Monitor;

            if (monitor == null)
            {
                return;
            }

            monitor.ClipboardChanged += HandleClipboardChanged;
        }

        private void HandleClipboardChanged(object sender, EventArgs e)
        {
            IClipboardItem item = null;

            if (System.Windows.Forms.Clipboard.ContainsText())
            {
                item = new ClipboardItem(System.Windows.Forms.Clipboard.GetText());
            }

            if (System.Windows.Forms.Clipboard.ContainsFileDropList())
            {
                item = new ClipboardItem(ToFileList(System.Windows.Forms.Clipboard.GetFileDropList()));
            }

            if (item == null
                || item.Hash == 0
                || item.Content.Length > 16384)
            {
                return;
            }

            SaveClipboardItem(item);
            ModifyClipboard(item);
        }

        private void ModifyClipboard(IClipboardItem item)
        {
            if (Configuration.SimplifyText && System.Windows.Forms.Clipboard.ContainsText())
            {
                System.Windows.Forms.Clipboard.Clear();
                System.Windows.Forms.Clipboard.SetText(item.Content);
            }
            else if(Configuration.ConvertFilesToPaths && System.Windows.Forms.Clipboard.ContainsFileDropList())
            {
                System.Windows.Forms.Clipboard.Clear();
                System.Windows.Forms.Clipboard.SetText(item.Content);
            }
        }

        private string ToFileList(StringCollection stringCollection)
        {
            StringBuilder stb = new StringBuilder();

            foreach (string s in stringCollection)
            {
                stb.AppendLine(s);
            }

            if (stb.Length > 0)
            {
                stb.Remove(stb.Length - Environment.NewLine.Length, Environment.NewLine.Length);
            }

            return stb.ToString();
        }

        private void SaveClipboardItem(IClipboardItem item)
        {
            if (history.Contains(item))
            {
                return;
            }

            history.Add(item);

            if (history.Count > HISTORY_SIZE)
            {
                IClipboardItem itemToDelete = history[0];
                historySet.Remove(itemToDelete);
                history.RemoveAt(0);
            }
        }

        private void DisposeMonitor()
        {
            if (monitor == null)
            {
                return;
            }

            monitor.ClipboardChanged -= HandleClipboardChanged;
            monitor = null;
        }
    }
}
