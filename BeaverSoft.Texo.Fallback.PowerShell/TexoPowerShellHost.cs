using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Text;
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

        private Runspace independentRunspace;
        private object independentLocker = new object(); 

        private Runspace pushedRunspace;
        private Stack<Guid> applications;
        private Stack<Guid> prompts;

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

            applications = new Stack<Guid>();
            prompts = new Stack<Guid>();
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

        public Runspace GetIndependentRunspace()
        {
            if (independentRunspace == null)
            {
                lock (independentLocker)
                {
                    if(independentRunspace == null)
                    {
                        independentRunspace = RunspaceFactory.CreateRunspace(this);
                        independentRunspace.Open();
                    }
                }
            }

            return independentRunspace;
        }

        public void Initialise()
        {
            Runspace = RunspaceFactory.CreateRunspace(this);
            Runspace.Open();
        }

        public override void EnterNestedPrompt()
        {
            prompts.Push(Guid.NewGuid());
        }

        public override void ExitNestedPrompt()
        {
            prompts.Pop();
        }

        public override void NotifyBeginApplication()
        {
            applications.Push(Guid.NewGuid());
            resultBuilder.SetRequireCustomErrorOutput();
        }

        public override void NotifyEndApplication()
        {
            applications.Pop();
        }

        public override void SetShouldExit(int exitCode)
        {
            Environment.Exit(exitCode);
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
