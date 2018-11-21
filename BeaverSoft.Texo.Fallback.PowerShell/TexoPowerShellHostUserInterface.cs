using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            throw new NotImplementedException();
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
            resultBuilder.Write(value, foregroundColor, backgroundColor);
        }

        public override void Write(string value)
        {
            resultBuilder.Write(value);
        }

        public override void WriteDebugLine(string message)
        {
            resultBuilder.WriteDebugLine(message);
        }

        public override void WriteErrorLine(string value)
        {
            resultBuilder.WriteErrorLine(value);
        }

        public override void WriteLine(string value)
        {
            resultBuilder.WriteLine(value);
        }

        public override void WriteProgress(
            long sourceId, ProgressRecord record)
        {
            throw new NotImplementedException();
        }

        public override void WriteVerboseLine(string message)
        {
            resultBuilder.WriteVerboseLine(message);
        }

        public override void WriteWarningLine(string message)
        {
            resultBuilder.WriteWarningLine(message);
        }
    }
}
