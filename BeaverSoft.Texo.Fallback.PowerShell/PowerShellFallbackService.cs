using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.Transforming;
using BeaverSoft.Texo.Core.View;
using BeaverSoft.Texo.Fallback.PowerShell.Transforming;
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

        private readonly IPipeline<InputModel> inputPipeline;
        private readonly PowerShellResultStreamBuilder resultBuilder;
        private readonly IPromptableViewService view;
        private readonly TexoPowerShellHost host;
        private readonly ILogService logger;

        private System.Management.Automation.PowerShell shell;
        private System.Management.Automation.PowerShell independentShell;

        public PowerShellFallbackService(
            IPromptableViewService view,
            ILogService logger)
        {
            inputPipeline = new Pipeline<InputModel>(logger);
            resultBuilder = new PowerShellResultStreamBuilder(logger);
            host = new TexoPowerShellHost(resultBuilder, view, logger);

            this.view = view;
            this.logger = logger;

            InitialiseInputPipeline();
        }

        private void InitialiseInputPipeline()
        {
            inputPipeline.AddPipe(new GitInput());
            inputPipeline.AddPipe(new DotnetInput());
            inputPipeline.AddPipe(new GetChildItemInput());
        }

        public async Task<ICommandResult> FallbackAsync(Input input)
        {
            if (input == null || input.ParsedInput.IsEmpty())
            {
                return new ErrorTextResult("Empty input.");
            }

            if (shell != null)
            {
                return new ErrorTextResult("A command is already in progress.");
            }

            BuildShell();

            try
            {
                InputModel inputModel = await inputPipeline.ProcessAsync(new InputModel(input));

                resultBuilder.Start(inputModel);

                _ = RunScriptToOutputAsync(inputModel.Command);
                return new TextStreamResult(resultBuilder.Stream);
            }
            catch (Exception e)
            {
                const string errorMessage = "Error during command execution in PowerShell.";
                logger.Error(errorMessage, e);
                resultBuilder.Stream?.Dispose();
                ReleaseShell();
                return new ErrorTextResult(errorMessage);
            }
        }

        public async Task<IEnumerable<string>> ProcessIndependentCommandAsync(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return Enumerable.Empty<string>();
            }

            int maxCount = 10;
            while (independentShell != null)
            {
                if (--maxCount < 0)
                {
                    return Enumerable.Empty<string>();
                }

                await Task.Delay(100);
            }

            BuildIndependentShell();

            try
            {
                return (await RunIndependentCommandAsync(input)).Select(obj => obj.BaseObject.ToString());
            }
            catch (Exception e)
            {
                logger.Error("Error during processing of command, quetly.", input, e);
                return Enumerable.Empty<string>();
            }
            finally
            {
                ReleaseIndependentShell();
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

        private Task<IEnumerable<PSObject>> RunIndependentCommandAsync(string input)
        {
            independentShell.Runspace = host.GetIndependentRunspace();
            independentShell.AddScript(input);

            return Task.Factory.FromAsync<IEnumerable<PSObject>>(independentShell.BeginInvoke(), (result) =>
            {
                return independentShell.EndInvoke(result);
            });
        }

        private Task RunScriptToOutputAsync(string script)
        {
            shell.Runspace = host.Runspace;
            shell.AddScript(script);
            shell.AddCommand("Out-Default");
            //shell.Commands.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);

            shell.Streams.Error.DataAdded += Error_DataAdded;
            shell.Streams.Warning.DataAdded += Warning_DataAdded;
            shell.Streams.Verbose.DataAdded += Verbose_DataAdded;
            shell.Streams.Debug.DataAdded += Debug_DataAdded;
            shell.Streams.Information.DataAdded += Information_DataAdded;

            return Task.Factory.FromAsync(shell.BeginInvoke(), (result) => 
            {
                //var output = shell.EndInvoke(result);
                resultBuilder.Finish();
                resultBuilder.Stream.NotifyAboutCompletion();

                shell.Streams.Error.DataAdded -= Error_DataAdded;
                shell.Streams.Warning.DataAdded -= Warning_DataAdded;
                shell.Streams.Verbose.DataAdded -= Verbose_DataAdded;
                shell.Streams.Debug.DataAdded -= Debug_DataAdded;
                shell.Streams.Information.DataAdded -= Information_DataAdded;
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

        private void Information_DataAdded(object sender, DataAddedEventArgs e)
        {
            var data = shell.Streams.Information[e.Index];
            resultBuilder.WriteVerboseLine(data.MessageData.ToString());
        }

        private void BuildShell()
        {
            lock (executionLock)
            {
                shell = System.Management.Automation.PowerShell.Create();
            }
        }

        private void BuildIndependentShell()
        {
            lock (executionLock)
            {
                independentShell = System.Management.Automation.PowerShell.Create();
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

        private void ReleaseIndependentShell()
        {
            lock (executionLock)
            {
                independentShell?.Dispose();
                independentShell = null;
            }
        }
    }
}
