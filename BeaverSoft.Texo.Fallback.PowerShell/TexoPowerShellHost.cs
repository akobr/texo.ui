using System;
using System.Globalization;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Fallback.PowerShell
{
    public class TexoPowerShellHost : PSHost, IHostSupportsInteractiveSession
    {
        private readonly IPromptableViewService view;
        private readonly ILogService logger;
        private Runspace pushedRunspace;

        public TexoPowerShellHost(IPromptableViewService view, ILogService logger)
        {
            this.view = view;
            this.logger = logger;

            InstanceId = Guid.NewGuid();
            Name = "Texo.UI PowerShell Fallback Host";
            Version = new Version(0, 9);
            CurrentCulture = CultureInfo.DefaultThreadCurrentCulture;
            CurrentUICulture = CultureInfo.DefaultThreadCurrentUICulture;

            UI = new TexoPowerShellHostUserInterface(view, logger);
        }

        public override CultureInfo CurrentCulture { get; }

        public override CultureInfo CurrentUICulture { get; }

        public override Guid InstanceId { get; }

        public override string Name { get; }

        public override PSHostUserInterface UI { get; }

        public override Version Version { get; }

        public bool IsRunspacePushed => pushedRunspace != null;

        public Runspace Runspace { get; private set; }

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
