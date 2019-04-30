using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Input.InputTree;
using BeaverSoft.Texo.Core.Path;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core.Messaging;
using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.Intellisense
{
    public class IntellisenseService : IIntellisenseService, IMessageBusService<IInputTreeUpdatedMessage>
    {
        private const int MAXIMUM_INPUT_SIZE = 512;
        private const int MAXIMUM_ITEMS_PER_TYPE = 20;

        private readonly ICommandManagementService commandManagement;
        private readonly IFallbackService fallback;
        private readonly IPathIntellisenseService pathIntellisense;
        private readonly IVariableIntellisenseService variableIntellisense;

        private TextumConfiguration configuration;
        private InputTree tree;

        public IntellisenseService(IEnvironmentService environment, ICommandManagementService commandManagement, IFallbackService fallback)
        {
            this.commandManagement = commandManagement;
            this.fallback = fallback;
            pathIntellisense = new PathIntellisenseService();
            variableIntellisense = new VariableIntellisenseService(environment);
        }

        public async Task<IImmutableList<IItem>> HelpAsync(Input.Input input, int cursorPosition)
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

            if (input.ParsedInput.Tokens.Count > 0)
            {
                if (string.Equals(input.ParsedInput.Tokens[0], "dotnet", StringComparison.OrdinalIgnoreCase))
                {
                    resultBuilder.AddRange(await BuildDotnetHelpAsync(input));
                }
            }

            if (!string.IsNullOrEmpty(input.Context.Key))
            {
                resultBuilder.AddRange(BuildCommandHelp(input, activeInput));
            }

            if (input.Tokens.Count > 0)
            {
                resultBuilder.AddRange(variableIntellisense.Help(activeInput).Take(MAXIMUM_ITEMS_PER_TYPE));
                resultBuilder.AddRange(pathIntellisense.Help(activeInput).Take(MAXIMUM_ITEMS_PER_TYPE));
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

                yield return Item.Intellisense(query.GetMainRepresentation(), "command", query.Documentation.Description);
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
                yield return Item.Intellisense(subQuery.GetMainRepresentation(), "query", subQuery.Documentation.Description);
            }

            foreach (Option subOption in query.Query.Options.OrderBy(o => o.Key))
            {
                string representation = subOption.GetMainRepresentation();
                yield return Item.Intellisense($"--{representation}", "option", subOption.Documentation.Description);
            }

            if (!commandManagement.HasCommand(input.Context.Key))
            {
                yield break;
            }

            ICommand command = commandManagement.BuildCommand(input.Context.Key);

            if (command is ISimpleIntellisenseSource intellisenseSource)
            {
                foreach (IItem item in intellisenseSource.GetHelp(activeInput).Take(MAXIMUM_ITEMS_PER_TYPE))
                {
                    yield return item;
                }
            }
        }

        private async Task<IEnumerable<IItem>> BuildDotnetHelpAsync(Input.Input input)
        {
            if (input.ParsedInput.RawInput.Length > 50
                || input.ParsedInput.RawInput.Contains('\"')
                || input.ParsedInput.RawInput.Contains('\''))
            {
                return Enumerable.Empty<IItem>();
            }

            string command = $"dotnet complete \"{string.Join(" ", input.ParsedInput.Tokens)} \"";
            var items = await fallback.ProcessIndependentCommandAsync(command);

            if (items == null)
            {
                return Enumerable.Empty<IItem>();
            }

            return items.Select(item => Item.Intellisense(item, item.StartsWith("-") ? "option" : "query", null));
        }

        void IMessageBusRecipient<IInputTreeUpdatedMessage>.ProcessMessage(IInputTreeUpdatedMessage message)
        {
            tree = message.InputTree;
            configuration = message.Configuration;
        }
    }
}
