using System;
using System.Management.Automation.Runspaces;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Fallback.PowerShell
{
    public class PowerShellFallbackService : IFallbackService
    {
        private readonly object executionLock = new object();
        private readonly IPromptableViewService view;
        private readonly ILogService logger;
        private readonly TexoPowerShellHost host;
        private readonly Runspace runspace;

        private System.Management.Automation.PowerShell shell;

        public PowerShellFallbackService(IPromptableViewService view, ILogService logger)
        {
            this.view = view;
            this.logger = logger;

            host = new TexoPowerShellHost(view, logger);
            runspace = RunspaceFactory.CreateRunspace(host);
        }
        public ICommandResult Fallback(Input input)
        {
            if (input == null || input.ParsedInput.IsEmpty())
            {
                return new ErrorTextResult("Empty input.");
            }

            if (shell != null)
            {
                return new ErrorTextResult("A command already in progress.");
            }

            lock (executionLock)
            {
                shell = System.Management.Automation.PowerShell.Create();
            }

            try
            {
                shell.Runspace = runspace;
                shell.AddScript(input.ParsedInput.RawInput);

                shell.AddCommand("out-default");
                shell.Commands.Commands[0].MergeMyResults(
                    PipelineResultTypes.Warning | PipelineResultTypes.Error,
                    PipelineResultTypes.Output);

                shell.Invoke();
                // TODO: build and return result item
                return new ItemsResult(Item.Plain("TODO: build and return result item"));
            }
            catch (Exception e)
            {
                logger.Error("Error during command execution in PowerShell.", e);
                return new ErrorTextResult(e.Message);
            }
            finally
            {
                lock (executionLock)
                {
                    shell?.Dispose();
                    shell = null;
                }
            }
        }

        public void Initialise()
        {
            runspace.Open();
        }
    }
}
