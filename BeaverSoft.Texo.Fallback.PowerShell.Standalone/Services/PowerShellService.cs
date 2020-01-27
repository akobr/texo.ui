using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.Transforming;
using BeaverSoft.Texo.Core.View;
using BeaverSoft.Texo.Fallback.PowerShell.Core;
using BeaverSoft.Texo.Fallback.PowerShell.Standalone.Communication;
using BeaverSoft.Texo.Fallback.PowerShell.Transforming;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.Services
{
    public class PowerShellService : IPowerShellService
    {
        private readonly object executionLock = new object();

        private readonly IPipeline<InputModel> inputPipeline;
        private readonly PipedPowerShellResultBuilder resultBuilder;
        private readonly TexoPowerShellHost host;

        private readonly IInputEvaluationService inputEvaluation;
        private readonly ICommunicator communicator;
        private readonly ILogService logger;

        private ConcurrentQueue<ICommandRequest> executionQueue;
        private System.Management.Automation.PowerShell shell;

        public PowerShellService(
            IInputEvaluationService inputEvaluation,
            IPromptableViewService view,
            ICommunicator communicator,
            ILogService logger)
        {
            executionQueue = new ConcurrentQueue<ICommandRequest>();
            inputPipeline = new Pipeline<InputModel>(logger);
            resultBuilder = new PipedPowerShellResultBuilder(communicator.Pipe.Messaging);
            host = new TexoPowerShellHost(resultBuilder, view, logger);

            this.inputEvaluation = inputEvaluation;
            this.communicator = communicator;
            this.logger = logger;

            InitialiseInputPipeline();
        }

        private void InitialiseInputPipeline()
        {
            inputPipeline.AddPipe(new GitInput());
            inputPipeline.AddPipe(new DotnetInput());
            inputPipeline.AddPipe(new GetChildItemInput());
        }

        public Task LastCommandExecutionTask { get; private set; }

        public bool IsCommandInProgress { get; private set; }

        public Guid CommandInProgressKey { get; private set; }

        public void Initialise()
        {
            host.Initialise();
        }

        public void AddCommandToQueue(ICommandRequest command)
        {
            executionQueue.Enqueue(command);

            if (!IsCommandInProgress)
            {
                _ = RunNextCommandAsync();
            }
        }

        public void CancelCommand(Guid commandKey)
        {
            if (IsCommandInProgress && CommandInProgressKey == commandKey)
            {
                shell?.Stop();
                return;
            }

            if (executionQueue.FirstOrDefault(cmd => cmd.Key == commandKey) is ICommandRequest command)
            {
                command.Cancel();
            }
        }

        public void Dispose()
        {
            ReleaseShell();
        }

        private async Task RunNextCommandAsync()
        {
            if (!TryDequeueCommand(out ICommandRequest command))
            {
                IsCommandInProgress = false;
                CommandInProgressKey = Guid.Empty;
                return;
            }

            try
            {
                if (command.IsCanceled)
                {
                    return;
                }

                IsCommandInProgress = true;
                CommandInProgressKey = command.Key;

                logger.Debug($"CommandStart: {command.Key:N}, {command.Input}");
                NativeConsole.WriteControlLine($"CommandStart:{command.Key:N}");
                Task executionTask = RunScriptToOutputAsync(command);
                LastCommandExecutionTask = executionTask;

                try
                {
                    await executionTask.ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    logger.Error("Error during command processing.", command.Input, exception);
                }

                NativeConsole.WriteControlLine($"CommandEnd:{command.Key:N}");
                logger.Debug($"CommandEnd: {command.Key:N}");
            }
            finally
            {
                _ = RunNextCommandAsync();
            }
        }

        private async Task RunScriptToOutputAsync(ICommandRequest command)
        {
            Task<Input> inputTask = Task.Run(() => inputEvaluation.Evaluate(command.Input));
            BuildShell();

            shell.Runspace = host.Runspace;
            InputModel inputModel = await inputPipeline.ProcessAsync(new InputModel(await inputTask));
            resultBuilder.SetCommandKey(command.Key);
            ValueTask startTask = resultBuilder.StartAsync(inputModel);

            shell.AddScript(command.Input);
            shell.AddCommand("Out-Default");

            shell.Streams.Error.DataAdded += OnErrorDataAdded;
            shell.Streams.Warning.DataAdded += OnWarningDataAdded;
            shell.Streams.Verbose.DataAdded += OnVerboseDataAdded;
            shell.Streams.Debug.DataAdded += OnDebugDataAdded;
            shell.Streams.Information.DataAdded += OnInformationDataAdded;

            await startTask;
            await Task.Factory.FromAsync(shell.BeginInvoke(), (result) =>
            {
                shell.Streams.Error.DataAdded -= OnErrorDataAdded;
                shell.Streams.Warning.DataAdded -= OnWarningDataAdded;
                shell.Streams.Verbose.DataAdded -= OnVerboseDataAdded;
                shell.Streams.Debug.DataAdded -= OnDebugDataAdded;
                shell.Streams.Information.DataAdded -= OnInformationDataAdded;
                ReleaseShell();
            });

            await resultBuilder.FinishAsync();
        }

        private void OnErrorDataAdded(object sender, DataAddedEventArgs e)
        {
            var data = shell.Streams.Error[e.Index];
            resultBuilder.WriteLineAsync(data.Exception.Message).AsTask().Wait();

            if (string.Equals(data.FullyQualifiedErrorId, "NativeCommandError", StringComparison.OrdinalIgnoreCase)
                || string.Equals(data.FullyQualifiedErrorId, "NativeCommandErrorMessage", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (data.InvocationInfo != null && !string.IsNullOrWhiteSpace(data.InvocationInfo.PositionMessage))
            {
                foreach (string line in data.InvocationInfo.PositionMessage.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                {
                    resultBuilder.WriteLineAsync(line, ConsoleColor.Red).AsTask().Wait();
                }
            }

            if (data.CategoryInfo != null)
            {
                resultBuilder.WriteLineAsync($"    + CategoryInfo: {data.CategoryInfo.Category}", ConsoleColor.Red).AsTask().Wait();
            }

            resultBuilder.WriteLineAsync($"    + FullyQualifiedErrorId: {data.FullyQualifiedErrorId}", ConsoleColor.Red).AsTask().Wait();
        }

        private void OnWarningDataAdded(object sender, DataAddedEventArgs e)
        {
            var data = shell.Streams.Warning[e.Index];
            resultBuilder.WriteLineAsync(data.Message, ConsoleColor.Yellow).AsTask().Wait();
        }

        private void OnVerboseDataAdded(object sender, DataAddedEventArgs e)
        {
            var data = shell.Streams.Verbose[e.Index];
            resultBuilder.WriteLineAsync(data.Message, ConsoleColor.Green).AsTask().Wait();
        }

        private void OnDebugDataAdded(object sender, DataAddedEventArgs e)
        {
            var data = shell.Streams.Debug[e.Index];
            resultBuilder.WriteLineAsync(data.Message, ConsoleColor.Magenta).AsTask().Wait();
        }

        private void OnInformationDataAdded(object sender, DataAddedEventArgs e)
        {
            var data = shell.Streams.Information[e.Index];
            resultBuilder.WriteLineAsync(data.MessageData.ToString(), ConsoleColor.Green).AsTask().Wait();
        }

        private bool TryDequeueCommand(out ICommandRequest command)
        {
            do
            {
                if (!executionQueue.TryDequeue(out command))
                {
                    return false;
                }

            } while (command.IsCanceled);

            return true;
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
