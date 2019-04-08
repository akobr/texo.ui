using System;
using System.Collections.Immutable;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core.Messaging;
using StrongBeaver.Core.Services;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Fallback.PowerShell
{
    public class PowerShellFallbackService :
        IFallbackService,
        IMessageBusService<IVariableUpdatedMessage>
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

            BuildShell();

            try
            {
                resultBuilder.StartItem();
                RunScriptToOutput(input.ParsedInput.RawInput);
                return new ItemsResult(
                    resultBuilder.ContainError
                        ? ResultTypeEnum.Failed
                        : ResultTypeEnum.Success,
                    ImmutableList.Create(resultBuilder.FinishItem()));
            }
            catch (Exception e)
            {
                const string errorMessage = "Error during command execution in PowerShell.";
                logger.Error(errorMessage, e);
                return new ErrorTextResult(errorMessage);
            }
            finally
            {
                ReleaseShell();
            }
        }

        public void Initialise()
        {
            host.Initialise();
            LoadPowerShellProfile();
        }

        void IMessageBusRecipient<IVariableUpdatedMessage>.ProcessMessage(IVariableUpdatedMessage message)
        {
            if (shell != null
                || message.Name != VariableNames.CURRENT_DIRECTORY)
            {
                return;
            }

            BuildShell();

            try
            {
                RunCommandQuetly("Set-Location", message.NewValue);
                shell.Invoke();
            }
            catch (Exception e)
            {
                logger.Error("Error during Set-Location execution in PowerShell.", message.NewValue, e);
            }
            finally
            {
                ReleaseShell();
            }
        }

        private void LoadPowerShellProfile()
        {
            BuildShell();

            try
            {
                shell.Runspace = host.Runspace;
                PSCommand[] profileCommands = HostUtilities.GetProfileCommands("TexoUI");

                foreach (PSCommand command in profileCommands)
                {
                    shell.Commands = command;
                    shell.Invoke();
                }
            }
            catch (Exception e)
            {
                logger.Error("Error during of profile loading in PowerShell.", e);
            }
            finally
            {
                ReleaseShell();
            }
        }

        private void RunCommandQuetly(string command, string argument)
        {
            shell.Runspace = host.Runspace;
            shell.AddCommand(command);
            shell.AddArgument(argument);
            shell.Invoke();
        }

        private void RunScriptToOutput(string script)
        {
            shell.Runspace = host.Runspace;
            shell.AddScript(script);
            shell.AddCommand("Out-Default");
            shell.Commands.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
            shell.Invoke();
        }

        private void BuildShell()
        {
            lock (executionLock)
            {
                shell = System.Management.Automation.PowerShell.Create();
            }
        }

        private void ReleaseShell()
        {
            lock (executionLock)
            {
                shell?.Dispose();
                shell = null;
            }
        }
    }
}
