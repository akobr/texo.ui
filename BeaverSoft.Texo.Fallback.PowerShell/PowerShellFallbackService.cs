using System;
using System.Management.Automation;
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
        private readonly IPowerShellResultBuilder resultBuilder;
        private readonly IPromptableViewService view;
        private readonly ILogService logger;
        private readonly TexoPowerShellHost host;

        private System.Management.Automation.PowerShell shell;

        public PowerShellFallbackService(
            IPowerShellResultBuilder resultBuilder,
            IPromptableViewService view,
            ILogService logger)
        {
            this.resultBuilder = resultBuilder;
            this.view = view;
            this.logger = logger;

            host = new TexoPowerShellHost(resultBuilder, view, logger);
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
                shell.Runspace = host.Runspace;
                shell.AddScript(input.ParsedInput.RawInput);

                //shell.AddCommand("write-host");
                shell.AddCommand("out-default");
                shell.Commands.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);

                resultBuilder.StartItem();
                var result = shell.Invoke();
                return new ItemsResult(resultBuilder.FinishItem());
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
            host.Initialise();
            LoadPowerShellProfile();
        }

        private void LoadPowerShellProfile()
        {
            lock (executionLock)
            {
                shell = System.Management.Automation.PowerShell.Create();
            }

            try
            {
                shell.Runspace = host.Runspace;

                PSCommand[] profileCommands = HostUtilities.GetProfileCommands("SampleHost06");
                foreach (PSCommand command in profileCommands)
                {
                    shell.Commands = command;
                    shell.Invoke();
                }
            }
            catch (Exception e)
            {
                logger.Error("Error during profile execution in PowerShell.", e);
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
    }
}
