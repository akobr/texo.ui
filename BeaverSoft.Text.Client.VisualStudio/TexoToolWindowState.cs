using BeaverSoft.Texo.Core;
using BeaverSoft.Texo.View.WPF;
using StrongBeaver.Core.Services;

namespace BeaverSoft.Text.Client.VisualStudio
{
    public class TexoToolWindowState
    {
        public EnvDTE80.DTE2 DTE { get; set; }

        public TexoEngine TexoEngine { get; set; }

        public WpfViewService View { get; set; }

        public IServiceMessageBus ServiceMessageBus { get; set; }
    }
}
