using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Console;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Fallback.PowerShell;
using BeaverSoft.Texo.Fallback.PowerShell.Core.Messages;
using BeaverSoft.Texo.Fallback.PowerShell.Standalone.Communication;
using BeaverSoft.Texo.Fallback.PowerShell.Standalone.NamedPipes;
using BeaverSoft.Texo.Fallback.PowerShell.Transforming;
using StrongBeaver.Core.Messaging;
using StrongBeaver.Core.Services;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Fallback.PseudoConsole
{
    public class PseudoConsoleFallbackService : IFallbackService, IMessageBusService<IVariableUpdatedMessage>, IDisposable
    {
        private readonly ICommunicator communicator;
        private readonly Terminal terminal;
        private readonly CancellationTokenSource terminalCancellationSource;
        private readonly PowerShellResultStreamBuilder resultBuilder;
        private readonly ILogService logger;

        private bool isCommandInProgress;
        private Guid commandKey;
        private CancellationTokenRegistration cancellationRegistration;

        public PseudoConsoleFallbackService(ILogService logger)
        {
            communicator = new Communicator(new NamedPipeServer(logger));
            terminal = new Terminal();
            terminalCancellationSource = new CancellationTokenSource();
            resultBuilder = new PowerShellResultStreamBuilder(logger);
            this.logger = logger;

            communicator.MessageReceived += OnMessageReceived;
        }

        public Task WatchTerminalTask { get; private set; }

        public async void Initialise()
        {
            string hostExePath = GetPowerShellHostPath();
            logger.Debug("Starting ConPTY with PowerShell host: " + hostExePath);
            //terminal.Start(hostExePath, 126, 64);
            await communicator.StartAsync();
            WatchTerminalTask = Task.Run(() => WatchTerminal(terminalCancellationSource.Token), terminalCancellationSource.Token);
            logger.Debug("ConPTY terminal fully started.");
        }

        public async Task<ICommandResult> FallbackAsync(Input input, CancellationToken cancellationToken = default)
        {
            if (input == null || input.ParsedInput.IsEmpty())
            {
                return new ErrorTextResult("Empty input.");
            }

            if (isCommandInProgress)
            {
                return new ErrorTextResult("A command is already in progress.");
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return new ErrorTextResult("Command has been cancelled.");
            }

            try
            {
                isCommandInProgress = true;
                commandKey = Guid.NewGuid();
                cancellationRegistration = cancellationToken.Register(OnCommandCancelled);
                await resultBuilder.StartAsync(new InputModel(input));
                await communicator.SendAsync(
                    new CommandMessage()
                    {
                        Key = commandKey,
                        Type = MessageType.RequestCommand,
                        Content = input.ParsedInput.RawInput
                    },
                    cancellationToken);
                return new TextStreamResult(resultBuilder.Stream);
            }
            catch (TaskCanceledException)
            {
                DisposeCommand();
                resultBuilder.Stream?.Dispose();
                return new ErrorTextResult("Command has been cancelled.");
            }
            catch (Exception e)
            {
                DisposeCommand();
                resultBuilder.Stream?.Dispose();
                const string errorMessage = "Error during command execution in PowerShell.";
                logger.Error(errorMessage, e);
                return new ErrorTextResult(errorMessage);
            }
        }

        public Task<IEnumerable<string>> ProcessIndependentCommandAsync(string input)
        {
            // TODO: implement independent commands
            return Task.FromResult(Enumerable.Empty<string>());
        }

        async void IMessageBusRecipient<IVariableUpdatedMessage>.ProcessMessage(IVariableUpdatedMessage message)
        {
            if (isCommandInProgress
                || message.Name != VariableNames.CURRENT_DIRECTORY)
            {
                return;
            }

            try
            {
                isCommandInProgress = true;
                commandKey = Guid.NewGuid();
                await resultBuilder.StartAsync(null);
                await communicator.SendAsync(new CommandMessage()
                {
                    Key = commandKey,
                    Type = MessageType.RequestCommand, // TODO: [P3] Should be quite command
                    Content = "Set-Location " + message.NewValue
                });
            }
            catch (Exception e)
            {
                DisposeCommand();
                resultBuilder.Stream?.Dispose();
                logger.Error("Error during Set-Location execution in PowerShell.", message.NewValue, e);
            }
        }

        private async Task WatchTerminal(CancellationToken cancellationToken)
        {
            using (StreamReader reader = new StreamReader(terminal.Output))
            using (var buffer = new PooledArray<char>(1024))
            {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    int readed = await reader.ReadAsync(buffer.Array, 0, buffer.Array.Length);

                    if (!isCommandInProgress)
                    {
                        continue;
                    }

                    await resultBuilder.WriteAsync(new string(buffer.Array, 0, readed));
                }
            }
        }

        private async void OnMessageReceived(RawMessage message)
        {
            CommandMessage cmdMessage = message.AsType<CommandMessage>();

            if (!isCommandInProgress || commandKey != cmdMessage.Key)
            {
                return;
            }

            switch (cmdMessage.Type)
            {
                case MessageType.CommandFinished:
                case MessageType.CommandCancelled:
                    DisposeCommand();
                    await resultBuilder.FinishAsync();
                    resultBuilder.Stream.NotifyAboutCompletion();
                    break;

                case MessageType.CommandOutput:
                    await resultBuilder.WriteAsync(cmdMessage.Content);
                    break;
            }
        }

        private string GetPowerShellHostPath()
        {
            string directory = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);

#if DEBUG
            // Debuging path to standalone poweshell project
            directory = Path.Combine(
                directory,
                "../../..",
                "BeaverSoft.Texo.Fallback.PowerShell.Standalone/bin/Debug");
#endif

            return Path.Combine(directory, "BeaverSoft.Texo.Fallback.PowerShell.Standalone.exe");
        }

        private void OnCommandCancelled()
        {
            if (!isCommandInProgress)
            {
                return;
            }

            DisposeCommand();
        }

        private void DisposeCommand()
        {
            isCommandInProgress = false;
            commandKey = Guid.Empty;
            cancellationRegistration.Dispose();
        }

        public void Dispose()
        {
            communicator.Dispose();
            terminal.Dispose();
        }
    }
}
