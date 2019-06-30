using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core.Messaging;
using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.Intellisense
{
    public class IntellisenseService : IIntellisenseService, IMessageBusService<IInputTreeUpdatedMessage>
    {
        private const int MAXIMUM_INPUT_SIZE = 512;
        private const int MAXIMUM_ITEMS_PER_TYPE = 20;

        private readonly CommandListIntellisenseProvider commandList;
        private readonly CommandDefinitionIntellisenseProvider commandDefinition;
        private readonly IIntellisenseProvider command;
        private readonly ITokenIntellisenseProvider path;
        private readonly ITokenIntellisenseProvider variable;

        private readonly Dictionary<string, IIntellisenseProvider> externalProviders;

        public IntellisenseService(IEnvironmentService environment, ICommandManagementService commandManagement)
        {
            externalProviders = new Dictionary<string, IIntellisenseProvider>(StringComparer.OrdinalIgnoreCase);

            commandDefinition = new CommandDefinitionIntellisenseProvider(commandManagement);
            commandList = new CommandListIntellisenseProvider();
            command = new CommandIntellisenseProvider(commandManagement);
            path = new PathIntellisenseProvider();
            variable = new VariableIntellisenseService(environment);
        }

        public void RegisterExternalHelpProvider(string command, IIntellisenseProvider provider)
        {
            externalProviders[command ?? string.Empty] = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public async Task<IImmutableList<IItem>> HelpAsync(Input input, int cursorPosition)
        {
            if (input?.ParsedInput == null
                || input.ParsedInput.IsEmpty())
            {
                return ImmutableList<IItem>.Empty.AddRange(commandList.GetHelp(input));
            }

            if (cursorPosition < input.ParsedInput.RawInput.Length
                || input.ParsedInput.RawInput.Length > MAXIMUM_INPUT_SIZE)
            {
                return ImmutableList<IItem>.Empty;
            }

            var resultBuilder = ImmutableList<IItem>.Empty.ToBuilder();
            string activeInputPart = cursorPosition > input.ParsedInput.RawInput.Length
                ? string.Empty : input.ParsedInput.Tokens[input.ParsedInput.Tokens.Count - 1];

            if (input.ParsedInput.Tokens.Count == 1
                && !string.IsNullOrEmpty(activeInputPart))
            {
                resultBuilder.AddRange(commandList.GetHelp(input));
            }

            if (input.ParsedInput.Tokens.Count > 1
                || string.IsNullOrEmpty(activeInputPart))
            {
                if (externalProviders.TryGetValue(input.ParsedInput.Tokens[0], out var provider))
                {
                    resultBuilder.AddRange(await provider.GetHelpAsync(input));
                }

                if (!string.IsNullOrEmpty(input.Context.Key))
                {
                    resultBuilder.AddRange(commandDefinition.GetHelp(input));
                    resultBuilder.AddRange(await command.GetHelpAsync(input));
                }
            }

            if (input.ParsedInput.Tokens.Count > 0)
            {
                resultBuilder.AddRange(variable.Help(activeInputPart).Take(MAXIMUM_ITEMS_PER_TYPE));
                resultBuilder.AddRange(path.Help(activeInputPart).Take(MAXIMUM_ITEMS_PER_TYPE));
            }

            return resultBuilder.ToImmutable();
        }

        void IMessageBusRecipient<IInputTreeUpdatedMessage>.ProcessMessage(IInputTreeUpdatedMessage message)
        {
            commandDefinition.SetTree(message.InputTree);
            commandList.SetConfiguration(message.Configuration);
        }
    }
}
