using System;
using BeaverSoft.Texo.Core;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.View.WPF;
using Microsoft.VisualStudio.ProjectSystem;
using StrongBeaver.Core.Services;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Text.Client.VisualStudio
{
    public class ExtensionContext
    {
        public ExtensionContext(
            EnvDTE80.DTE2 dte,
            IProjectThreadingService threading,
            TexoEngine texoEngine,
            IServiceMessageBus messageBus)
        {
            DTE = dte ?? throw new ArgumentNullException(nameof(dte));
            Threading = threading ?? throw new ArgumentNullException(nameof(threading));
            MessageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            TexoEngine = texoEngine ?? throw new ArgumentNullException(nameof(texoEngine));
            View = (WpfViewService)texoEngine.View;
            Logger = texoEngine.Logger;
        }

        public EnvDTE80.DTE2 DTE { get; }

        public IProjectThreadingService Threading { get; }

        public TexoEngine TexoEngine { get; }

        public IExecutor Executor { get; }

        public WpfViewService View { get; }

        public IServiceMessageBus MessageBus { get; }

        public ILogService Logger { get; }
    }
}
