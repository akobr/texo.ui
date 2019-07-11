using System;
using System.Linq;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Model.Text;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Help
{
    public class HelpCommand : ModularCommand
    {
        private readonly ISettingService setting;

        public HelpCommand(ISettingService setting)
        {
            this.setting = setting ?? throw new ArgumentNullException(nameof(setting));

            RegisterQuery(HelpNames.QUERY_INFO, Info);
            RegisterQuery(HelpNames.QUERY_TREE, Tree);
            RegisterQuery(HelpNames.QUERY_LIST, List);
        }

        private ICommandResult Info(CommandContext context)
        {
            string commandKey = context.GetParameterValue(ParameterKeys.ITEM);
            string input = string.Empty;

            Query query = FindCommand(commandKey);
            Option option = null;
            Parameter parameter = null;

            if (string.IsNullOrEmpty(commandKey))
            {
                return List(context);
            }

            if (query == null)
            {
                return new ErrorTextResult($"The command '{commandKey}' hasn't been found.");
            }

            foreach (string inputItem in context.GetParameterValues(ParameterKeys.ITEM).Skip(1))
            {
                if (option != null)
                {
                    parameter = option.Parameters.FirstOrDefault(p =>
                        string.Equals(p.Key, inputItem, StringComparison.OrdinalIgnoreCase));
                    break;
                }

                var nextQuery = query.Queries.FirstOrDefault(q =>
                    string.Equals(q.Key, inputItem, StringComparison.OrdinalIgnoreCase));

                if (nextQuery != null)
                {
                    input += query.GetMainRepresentation() + " ";
                    query = nextQuery;
                    continue;
                }

                option = query.Options.FirstOrDefault(o =>
                    string.Equals(o.Key, inputItem, StringComparison.OrdinalIgnoreCase));

                if (option != null)
                {
                    continue;
                }

                parameter = query.Parameters.FirstOrDefault(p =>
                    string.Equals(p.Key, inputItem, StringComparison.OrdinalIgnoreCase));
                break;
            }

            input = input.Trim();

            if (parameter != null)
            {
                return new ModeledResult(BuildParameterInfo(parameter));
            }

            if (option != null)
            {
                return new ModeledResult(BuildOptionInfo(option));
            }

            if (query != null)
            {
                return new ModeledResult(BuildQueryInfo(query, input, true));
            }

            return new ErrorTextResult($"Unknown input: {input}");
        }

        private ICommandResult Tree(CommandContext context)
        {
            string commandKey = context.GetParameterValue(HelpNames.PARAMETER_COMMAND);
            Query command = FindCommand(commandKey);

            if (command == null)
            {
                return new ErrorTextResult($"The command '{commandKey}' hasn't been found.");
            }

            CommandTextTreeBuilder builder = new CommandTextTreeBuilder(context.HasOption(HelpOptions.TEMPLATE));
            string tree = builder.BuildTree(command);
            return new ItemsResult(Item.AsPlain(tree));
        }

        private ICommandResult List(CommandContext context)
        {
            List result = new List();

            foreach (Query command in setting.Configuration.Runtime.Commands.OrderBy(c => c.Key))
            {
                string commandName = command.GetMainRepresentation();

                result = result.AddItem(new ListItem(
                    new Span(
                        new Strong(
                            new Model.Text.Link(commandName, ActionBuilder.CommandRunUri($"help info {commandName}"))),
                        new PlainText(": "),
                        new Italic(command.Documentation.Description))));
            }

            return new ModeledResult(result);
        }

        private Query FindCommand(string commandKey)
        {
            return setting.Configuration.Runtime.Commands.FirstOrDefault(c => c.Representations.Contains(commandKey, StringComparer.InvariantCultureIgnoreCase));
        }

        private static IBlock BuildQueryInfo(Query query, string input, bool listChildren)
        {
            input += " " + query.GetMainRepresentation();
            input = input.Trim();

            Section result = new Section(
                new Header(query.Documentation?.Title ?? query.GetMainRepresentation()),
                new Paragraph(
                    new Model.Text.Link(input, ActionBuilder.CommandRunUri($"help info {input}"))));

            if (!string.IsNullOrEmpty(query.Documentation?.Description))
            {
                result = result.AddChild(new Paragraph(query.Documentation.Description));
            }

            if (!listChildren)
            {
                return result;
            }

            if (query.Queries.Count > 0)
            {
                result = result.AddChild(BuildQueriesSection(query, input));
            }

            if (query.Options.Count > 0)
            {
                result = result.AddChild(BuildOptionsSection(query));
            }

            if (query.Parameters.Count > 0)
            {
                result = result.AddChild(BuildParametersSection(query));
            }

            return result;
        }

        private static IBlock BuildOptionInfo(Option option)
        {
            Section result = new Section(
                new Header(option.Documentation?.Title ?? option.GetMainRepresentation()));
            ISpanBuilder content = new SpanBuilder();

            foreach (string representation in option.Representations)
            {
                using (content.StartStrongContext())
                {
                    string optionInput = representation.Length > 2
                        ? $"--{representation}"
                        : $"-{representation}";

                    content.Link(optionInput, ActionBuilder.InputAddUri(optionInput));
                }

                content.WriteLine();
            }

            result = result.AddChild(new Paragraph(content.Span));

            if (!string.IsNullOrEmpty(option.Documentation?.Description))
            {
                result = result.AddChild(new Paragraph(option.Documentation.Description));
            }

            if (option.Parameters.Count > 0)
            {
                result = result.AddChild(BuildParametersSection(option));
            }

            return result;
        }

        private static IBlock BuildParameterInfo(Parameter parameter)
        {
            Section result = new Section(
                new Header(parameter.Documentation.Title ?? parameter.Key));

            if (!string.IsNullOrEmpty(parameter.ArgumentTemplate))
            {
                result = result.AddChild(new Paragraph(
                    SpanBuilder.Create()
                        .Write("Regular expression: ")
                        .Italic($"/{parameter.ArgumentTemplate}/")));
            }

            if (!string.IsNullOrEmpty(parameter.Documentation?.Description))
            {
                result = result.AddChild(new Paragraph(parameter.Documentation.Description));
            }

            return result;
        }

        private static IBlock BuildQueriesSection(Query query, string input)
        {
            Section result = new Section(new Header("Sub-Commands"));

            foreach (Query subQuery in query.Queries.OrderBy(q => q.Key))
            {
                result = result.AddChild(BuildQueryInfo(subQuery, input, false));
            }

            return result;
        }

        private static IBlock BuildOptionsSection(Query query)
        {
            Section result = new Section(new Header("Options"));

            foreach (Option option in query.Options.OrderBy(o => o.Key))
            {
                result = result.AddChild(BuildOptionInfo(option));
            }

            return result;
        }

        private static IBlock BuildParametersSection(InputStatement statement)
        {
            Section result = new Section(new Header("Parameters"));

            foreach (Parameter parameter in statement.Parameters)
            {
                result = result.AddChild(BuildParameterInfo(parameter));
            }

            return result;
        }
    }
}