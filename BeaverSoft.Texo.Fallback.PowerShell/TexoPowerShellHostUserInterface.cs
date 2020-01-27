using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;
using BeaverSoft.Texo.Core.View;
using BeaverSoft.Texo.Fallback.PowerShell.Extensions;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Fallback.PowerShell
{
    class TexoPowerShellHostUserInterface : PSHostUserInterface, IHostUISupportsMultipleChoiceSelection
    {
        private readonly IPowerShellResultBuilder resultBuilder;
        private readonly IPromptableViewService view;
        private readonly ILogService logger;

        public TexoPowerShellHostUserInterface(
            IPowerShellResultBuilder resultBuilder,
            IPromptableViewService view,
            ILogService logger)
        {
            this.resultBuilder = resultBuilder;
            this.view = view;
            this.logger = logger;

            RawUI = new TexoPowerShellHostRawUserInterface(logger);
        }

        public override PSHostRawUserInterface RawUI { get; }

        public override Dictionary<string, PSObject> Prompt(
            string caption, string message, Collection<FieldDescription> descriptions)
        {
            resultBuilder.WriteLineAsync($"PowerShell expect the parameter(s):");

            if (descriptions != null && descriptions.Count > 0)
            {
                resultBuilder.WriteLineAsync(string.Join(", ", descriptions.Select(f => f.Name)));
            }

            if (string.IsNullOrWhiteSpace(caption))
            {
                resultBuilder.WriteLineAsync(caption);
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                resultBuilder.WriteLineAsync(message);
            }

            return new Dictionary<string, PSObject>();
        }

        public override int PromptForChoice(
            string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
        {
            throw new NotImplementedException();
        }

        public Collection<int> PromptForChoice(
            string caption, string message, Collection<ChoiceDescription> choices, IEnumerable<int> defaultChoices)
        {
            throw new NotImplementedException();
        }

        public override PSCredential PromptForCredential(
            string caption, string message, string userName, string targetName)
        {
            throw new NotImplementedException();
        }

        public override PSCredential PromptForCredential(
            string caption, string message, string userName, string targetName,
            PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
        {
            throw new NotImplementedException();
        }

        public override string ReadLine()
        {
            return view.GetNewInput();
        }

        public override SecureString ReadLineAsSecureString()
        {
            return ReadLine().ToSecureString();
        }

        public override void Write(
            ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            resultBuilder.WriteAsync(value, foregroundColor, backgroundColor);
        }

        public override void Write(string value)
        {
            resultBuilder.WriteAsync(value);
        }

        public override void WriteDebugLine(string message)
        {
            resultBuilder.WriteLineAsync(message, ConsoleColor.Magenta);
        }

        public override void WriteErrorLine(string value)
        {
             resultBuilder.WriteLineAsync(value, ConsoleColor.Red);
        }

        public override void WriteLine(string value)
        {
            resultBuilder.WriteLineAsync(value);
        }

        public override void WriteLine(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            resultBuilder.WriteLineAsync(value, foregroundColor, backgroundColor);
        }

        public override void WriteLine()
        {
            resultBuilder.WriteLineAsync();
        }

        public override void WriteInformation(InformationRecord record)
        {
            if (!(record?.MessageData is HostInformationMessage message) || message.NoNewLine != true)
            {
                base.WriteInformation(record);
                return;
            }

            // When the script contains 'Write-Host -NoNewLine [MESSAGE]',
            // we are expecting update message which will override current line.
            resultBuilder.WriteAsync(
                    '\r' + message.Message,
                    message.ForegroundColor ?? ConsoleColor.White,
                    message.BackgroundColor ?? ConsoleColor.Black);
        }

        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
            view.ShowProgress(record.ActivityId, record.Activity ?? "operation", record.PercentComplete);
            logger.Debug(record.Activity, record.StatusDescription, record.CurrentOperation);
        }

        public override void WriteVerboseLine(string message)
        {
            resultBuilder.WriteLineAsync(message, ConsoleColor.Green);
        }

        public override void WriteWarningLine(string message)
        {
            resultBuilder.WriteLineAsync(message, ConsoleColor.Yellow);
        }
    }
}
