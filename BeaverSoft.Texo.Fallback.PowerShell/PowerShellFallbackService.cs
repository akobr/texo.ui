using System;
using System.Collections.Immutable;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.Streaming.Text;
using BeaverSoft.Texo.Core.Transforming;
using BeaverSoft.Texo.Core.View;
using BeaverSoft.Texo.Fallback.PowerShell.Git;
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

        private readonly PowerShellResultStreamBuilder resultBuilder;
        private readonly IPromptableViewService view;
        private readonly ILogService logger;
        private readonly TexoPowerShellHost host;

        private System.Management.Automation.PowerShell shell;

        public PowerShellFallbackService(
            IPromptableViewService view,
            ILogService logger)
        {
            resultBuilder = new PowerShellResultStreamBuilder();

            this.view = view;
            this.logger = logger;

            host = new TexoPowerShellHost(resultBuilder, view, logger);
        }

        public Task<ICommandResult> FallbackAsync(Input input)
        {
            if (input == null || input.ParsedInput.IsEmpty())
            {
                return Task.FromResult<ICommandResult>(new ErrorTextResult("Empty input."));
            }

            if (shell != null)
            {
                return Task.FromResult<ICommandResult>(new ErrorTextResult("A command already in progress."));
            }

            BuildShell();

            try
            {
                // TODO: [P3] move this to pipeline 
                string commandText = input.ParsedInput.RawInput;
                if (string.Equals(input.ParsedInput.Tokens[0], "git", StringComparison.OrdinalIgnoreCase))
                {
                    commandText = "git -c color.ui=always -c color.status=always -c color.branch=always -c color.diff=always -c color.interactive=always ";
                    commandText += string.Join(" ", input.ParsedInput.Tokens.Skip(1));
                }

                resultBuilder.StartItem();
                Task outputTask = RunScriptToOutputAsync(commandText);
                return Task.FromResult<ICommandResult>(new TextStreamResult(resultBuilder.Stream, outputTask));
            }
            catch (Exception e)
            {
                const string errorMessage = "Error during command execution in PowerShell.";
                logger.Error(errorMessage, e);
                resultBuilder.Stream?.Dispose();
                ReleaseShell();
                return Task.FromResult<ICommandResult>(new ErrorTextResult(errorMessage));
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
                // Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
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

        private Task RunScriptToOutputAsync(string script)
        {
            shell.Runspace = host.Runspace;
            shell.AddScript(script);
            shell.AddCommand("Out-Default");
            //shell.Commands.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
            return Task.Factory.FromAsync(shell.BeginInvoke(), (result) => 
            {
                var output = shell.EndInvoke(result);
                resultBuilder.Stream.NotifyAboutCompletion();
                ReleaseShell();
            });
        }

        private void Error_DataAdded(object sender, DataAddedEventArgs e)
        {
            var data = shell.Streams.Error[e.Index];
            resultBuilder.WriteErrorLine(data.Exception.Message);

            if (string.Equals(data.FullyQualifiedErrorId, "NativeCommandError", StringComparison.OrdinalIgnoreCase)
                || string.Equals(data.FullyQualifiedErrorId, "NativeCommandErrorMessage", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (data.InvocationInfo != null && !string.IsNullOrWhiteSpace(data.InvocationInfo.PositionMessage))
            {
                foreach (string line in data.InvocationInfo.PositionMessage.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                {
                    resultBuilder.WriteErrorLine(line);
                }
            }

            if (data.CategoryInfo != null)
            {
                resultBuilder.WriteErrorLine($"    + CategoryInfo: {data.CategoryInfo.Category}");
            }

            resultBuilder.WriteErrorLine($"    + FullyQualifiedErrorId: {data.FullyQualifiedErrorId}");
        }

        private void Warning_DataAdded(object sender, DataAddedEventArgs e)
        {
            var data = shell.Streams.Warning[e.Index];
            resultBuilder.WriteWarningLine(data.Message);
        }

        private void Verbose_DataAdded(object sender, DataAddedEventArgs e)
        {
            var data = shell.Streams.Verbose[e.Index];
            resultBuilder.WriteVerboseLine(data.Message);
        }

        private void Debug_DataAdded(object sender, DataAddedEventArgs e)
        {
            var data = shell.Streams.Debug[e.Index];
            resultBuilder.WriteDebugLine(data.Message);
        }

        private void BuildShell()
        {
            lock (executionLock)
            {
                shell = System.Management.Automation.PowerShell.Create();

                shell.Streams.Error.DataAdded += Error_DataAdded;
                shell.Streams.Warning.DataAdded += Warning_DataAdded;
                shell.Streams.Verbose.DataAdded += Verbose_DataAdded;
                shell.Streams.Debug.DataAdded += Debug_DataAdded;
            }
        }

        private void ReleaseShell()
        {
            lock (executionLock)
            {
                shell.Streams.Error.DataAdded -= Error_DataAdded;
                shell.Streams.Warning.DataAdded -= Warning_DataAdded;
                shell.Streams.Verbose.DataAdded -= Verbose_DataAdded;
                shell.Streams.Debug.DataAdded -= Debug_DataAdded;

                shell?.Dispose();
                shell = null;
            }
        }
    }
}
