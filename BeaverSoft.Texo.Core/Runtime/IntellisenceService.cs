using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Input.InputTree;
using BeaverSoft.Texo.Core.Path;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core.Messaging;
using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.Runtime
{
    public class IntellisenceService : IIntellisenceService, IMessageBusService<IInputTreeUpdatedMessage>
    {
        private const int MAXIMUM_INPUT_SIZE = 512;
        private const int MAXIMUM_ITEMS_PER_TYPE = 20;

        private readonly ICommandManagementService commandManagement;
        private readonly IPathIntellisenceService pathIntellisence;
        private readonly IVariableIntellisenceService variableIntellisence;

        private TextumConfiguration configuration;
        private InputTree tree;

        public IntellisenceService(IEnvironmentService environment, ICommandManagementService commandManagement)
        {
            this.commandManagement = commandManagement;
            pathIntellisence = new PathIntellisenceService();
            variableIntellisence = new VariableIntellisenceService(environment);
        }

        public IImmutableList<IItem> Help(Input.Input input, int cursorPosition)
        {
            if (input?.ParsedInput == null
                || input.ParsedInput.IsEmpty())
            {
                return ImmutableList<IItem>.Empty.AddRange(BuildCommandList(string.Empty));
            }

            if (cursorPosition < input.ParsedInput.RawInput.Length
                || input.ParsedInput.RawInput.Length > MAXIMUM_INPUT_SIZE)
            {
                return ImmutableList<IItem>.Empty;
            }

            var resultBuilder = ImmutableList<IItem>.Empty.ToBuilder();

            var lastToken = input.Tokens[input.Tokens.Count - 1];
            string activeInput = cursorPosition > input.ParsedInput.RawInput.Length
                ? string.Empty
                : input.ParsedInput.RawInput.Substring(input.ParsedInput.RawInput.LastIndexOf(" ") + 1);

            if (input.Tokens.Count == 1 && !string.IsNullOrEmpty(activeInput))
            {
                resultBuilder.AddRange(BuildCommandList(lastToken.Input));
            }
            else if (!string.IsNullOrEmpty(input.Context.Key))
            {
                resultBuilder.AddRange(BuildCommandHelp(input, activeInput));
            }

            if (input.Tokens.Count > 0)
            {
                resultBuilder.AddRange(variableIntellisence.Help(activeInput).Take(MAXIMUM_ITEMS_PER_TYPE));
                resultBuilder.AddRange(pathIntellisence.Help(activeInput).Take(MAXIMUM_ITEMS_PER_TYPE));
            }

            return resultBuilder.ToImmutable();
        }

        public void Initialise(InputTree newTree)
        {
            tree = newTree;
        }

        private IEnumerable<IItem> BuildCommandList(string commandTerm)
        {
            foreach (Query query in configuration.Runtime.Commands.OrderBy(cmd => cmd.Key))
            {
                if (!query.Representations.Any(r => r.StartsWith(commandTerm, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                yield return Item.Intellisence(query.GetMainRepresentation(), "command", query.Documentation.Description);
            }
        }

        private IEnumerable<IItem> BuildCommandHelp(Input.Input input, string activeInput)
        {
            QueryNode query = tree.Root;

            foreach (Token token in input.Tokens)
            {
                if (token.Type != TokenTypeEnum.Query 
                    || !query.Queries.ContainsKey(token.Input))
                {
                    break;
                }

                query = query.Queries[token.Input];
            }

            foreach (Query subQuery in query.Query.Queries.OrderBy(q => q.Key))
            {
                yield return Item.Intellisence(subQuery.GetMainRepresentation(), "query", subQuery.Documentation.Description);
            }

            foreach (Option subOption in query.Query.Options.OrderBy(o => o.Key))
            {
                string representation = subOption.GetMainRepresentation();
                yield return Item.Intellisence(representation, $"--{representation}", "option", subOption.Documentation.Description);

            }

            if (!commandManagement.HasCommand(input.Context.Key))
            {
                yield break;
            }

            ICommand command = commandManagement.BuildCommand(input.Context.Key);

            if (command is ISimpleIntellisenceSource intellisenceSource)
            {
                foreach (IItem item in intellisenceSource.GetHelp(activeInput).Take(MAXIMUM_ITEMS_PER_TYPE))
                {
                    yield return item;
                }
            }
        }

        void IMessageBusRecipient<IInputTreeUpdatedMessage>.ProcessMessage(IInputTreeUpdatedMessage message)
        {
            tree = message.InputTree;
            configuration = message.Configuration;
        }
    }
}
