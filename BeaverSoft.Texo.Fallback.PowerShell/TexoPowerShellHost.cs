using System;
using System.Globalization;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Threading;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Fallback.PowerShell
{
    public class TexoPowerShellHost : PSHost, IHostSupportsInteractiveSession, IInitialisable
    {
        private readonly IPowerShellResultBuilder resultBuilder;
        private readonly IPromptableViewService view;
        private readonly ILogService logger;
        private Runspace pushedRunspace;

        public TexoPowerShellHost(
            IPowerShellResultBuilder resultBuilder,
            IPromptableViewService view,
            ILogService logger)
        {
            this.resultBuilder = resultBuilder;
            this.view = view;
            this.logger = logger;

            InstanceId = Guid.NewGuid();
            Name = "Texo.UI PowerShell Fallback Host";
            Version = new Version(1, 0);
            CurrentCulture = Thread.CurrentThread.CurrentCulture;
            CurrentUICulture = Thread.CurrentThread.CurrentUICulture;

            UI = new TexoPowerShellHostUserInterface(resultBuilder, view, logger);
        }

        public override CultureInfo CurrentCulture { get; }

        public override CultureInfo CurrentUICulture { get; }

        public override Guid InstanceId { get; }

        public override string Name { get; }

        public override PSHostUserInterface UI { get; }

        public override Version Version { get; }

        public bool IsRunspacePushed => pushedRunspace != null;

        public Runspace Runspace { get; private set; }

        public void Initialise()
        {
            Runspace = RunspaceFactory.CreateRunspace(this);
            Runspace.Open();
        }

        public override void EnterNestedPrompt()
        {
            throw new NotImplementedException();
        }

        public override void ExitNestedPrompt()
        {
            throw new NotImplementedException();
        }

        public override void NotifyBeginApplication()
        {
            throw new NotImplementedException();
        }

        public override void NotifyEndApplication()
        {
            throw new NotImplementedException();
        }

        public override void SetShouldExit(int exitCode)
        {
            throw new NotImplementedException();
        }

        public void PopRunspace()
        {
            Runspace = pushedRunspace;
            pushedRunspace = null;
        }

        public void PushRunspace(Runspace runspace)
        {
            pushedRunspace = Runspace;
            Runspace = runspace;
        }
    }
}
