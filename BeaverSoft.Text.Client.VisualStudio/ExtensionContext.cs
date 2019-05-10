using System;
using BeaverSoft.Texo.Core;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.View.WPF;
using Microsoft.VisualStudio.Threading;
using StrongBeaver.Core.Services;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Text.Client.VisualStudio
{
    public class ExtensionContext
    {
        public ExtensionContext(
            EnvDTE80.DTE2 dte,
            JoinableTaskFactory taskFactory,
            TexoEngine texoEngine,
            IServiceMessageBus messageBus)
        {
            DTE = dte ?? throw new ArgumentNullException(nameof(dte));
            TaskFactory = taskFactory ?? throw new ArgumentNullException(nameof(taskFactory));
            MessageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            TexoEngine = texoEngine ?? throw new ArgumentNullException(nameof(texoEngine));
            View = (WpfViewService)texoEngine.View;
            Executor = texoEngine.Runtime;
            Logger = texoEngine.Logger;
        }

        public EnvDTE80.DTE2 DTE { get; }

        public JoinableTaskFactory TaskFactory { get; }

        public TexoEngine TexoEngine { get; }

        public IExecutor Executor { get; }

        public WpfViewService View { get; }

        public IServiceMessageBus MessageBus { get; }

        public ILogService Logger { get; }
    }
}
