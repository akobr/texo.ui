using System;
using System.Linq;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Model.Text;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Help
{
    public class HelpCommand : InlineIntersectionCommand
    {
        private readonly ISettingService setting;

        public HelpCommand(ISettingService setting)
        {
            this.setting = setting ?? throw new ArgumentNullException(nameof(setting));

            RegisterQueryMethod(HelpNames.QUERY_INFO, Info);
            RegisterQueryMethod(HelpNames.QUERY_TREE, Tree);
            RegisterQueryMethod(HelpNames.QUERY_LIST, List);
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

            foreach (string inputItem in context.GetParameterValues(ParameterKeys.ITEM))
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
                    query = nextQuery;
                    continue;
                }

                option = query.Options.FirstOrDefault(o =>
                    string.Equals(o.Key, inputItem, StringComparison.OrdinalIgnoreCase));

                if (option == null)
                {
                    break;
                }
            }

            if (parameter != null)
            {
                return BuildParameterInfo(parameter);
            }

            if (option != null)
            {
                return BuildOptionInfo(option);
            }

            if (query != null)
            {
                return BuildQueryInfo(query);
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

            CommandTextTreeBuilder builder = new CommandTextTreeBuilder();
            string tree = builder.BuildTree(command);
            return new ItemsResult(Item.Plain(tree));
        }

        private ICommandResult List(CommandContext context)
        {
            List result = new List();

            foreach (Query command in setting.Configuration.Runtime.Commands.OrderBy(c => c.Key))
            {
                result = result.AddItem(new ListItem(
                    new Span(
                        new Strong(command.GetMainRepresentation()),
                        new PlainText(" - "),
                        new Italic(command.Documentation.Description))));
            }

            return new ItemsResult<ModeledItem>(new ModeledItem(result));
        }

        private Query FindCommand(string commandKey)
        {
            return setting.Configuration.Runtime.Commands.FirstOrDefault(c => c.Representations.Contains(commandKey, StringComparer.InvariantCultureIgnoreCase));
        }

        private static ICommandResult BuildQueryInfo(Query query)
        {
            Document document = new Document(
                new Header(query.Documentation.Title ?? query.GetMainRepresentation()),
                new Paragraph(query.Documentation.Description));

            return new ItemsResult<ModeledItem>(new ModeledItem(document));
        }

        private static ICommandResult BuildOptionInfo(Option option)
        {
            Document document = new Document(
                new Header(option.Documentation.Title ?? option.GetMainRepresentation()),
                new Paragraph(option.Documentation.Description));

            return new ItemsResult<ModeledItem>(new ModeledItem(document));
        }

        private static ICommandResult BuildParameterInfo(Parameter parameter)
        {
            Document document = new Document(
                new Header(parameter.Documentation.Title ?? parameter.Key),
                new Paragraph(parameter.Documentation.Description));

            return new ItemsResult<ModeledItem>(new ModeledItem(document));
        }
    }
}