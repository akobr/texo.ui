using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Help;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Input.History;

namespace BeaverSoft.Texo.Core
{
    public static class CommandsBuilder
    {
        public static Query BuildCurrentDirectory()
        {
            var command = Query.CreateBuilder();

            var parameter = Parameter.CreateBuilder();
            parameter.Key = ParameterKeys.PATH;
            parameter.ArgumentTemplate = InputRegex.PATH;
            parameter.IsOptional = true;
            parameter.IsRepeatable = true;
            parameter.Documentation.Title = "Directory path";
            parameter.Documentation.Description = "Specify full or relative path(s) to working directory.";

            command.Key = CommandKeys.CURRENT_DIRECTORY;
            command.Representations.AddRange(
                new[] { CommandKeys.CURRENT_DIRECTORY, "cd", "chdir", "cdir" });
            command.Parameters.Add(parameter.ToImmutable());
            command.Documentation.Title = "Current directory";
            command.Documentation.Description = "Gets or sets current working directory.";

            return command.ToImmutable();
        }

        public static Query BuildClear()
        {
            var command = Query.CreateBuilder();

            command.Key = CommandKeys.CLEAR;
            command.Representations.AddRange(
                new[] { CommandKeys.CLEAR, "cls", "Clear-Host", "gc" });
            command.Documentation.Title = "Clear output widnow";
            command.Documentation.Description = "All previous outputs will be clear.";

            return command.ToImmutable();
        }

        public static Query BuildHelp()
        {
            var command = Query.CreateBuilder();

            command.Key = CommandKeys.HELP;
            command.Representations.AddRange(
                new[] { CommandKeys.HELP, "advice", "aid" });
            command.DefaultQueryKey = HelpNames.QUERY_INFO;
            command.Documentation.Title = "Help";
            command.Documentation.Description = "Help about available commands.";

            var parQuery = Parameter.CreateBuilder();
            parQuery.Key = ParameterKeys.ITEM;
            parQuery.ArgumentTemplate = InputRegex.QUERY_OR_OPTION;
            parQuery.IsOptional = true;
            parQuery.IsRepeatable = true;
            parQuery.Documentation.Title = "Query";
            parQuery.Documentation.Description = "Specify command / query / option or parameter name about which you want to know more.";

            var parCommand = Parameter.CreateBuilder();
            parCommand.Key = HelpNames.PARAMETER_COMMAND;
            parCommand.ArgumentTemplate = InputRegex.VARIABLE_NAME;
            parCommand.Documentation.Title = "Command name";
            parCommand.Documentation.Description = "Specify command name for which will be generated input tree.";

            var optionTemplate = Option.CreateBuilder();
            optionTemplate.Key = HelpOptions.TEMPLATE;
            optionTemplate.Representations.AddRange(
                new[] { HelpOptions.TEMPLATE, "t" });
            optionTemplate.Documentation.Title = "Parameters template";
            optionTemplate.Documentation.Description = "Show parameters template (regular expression).";

            var queryInfo = Query.CreateBuilder();
            queryInfo.Key = HelpNames.QUERY_INFO;
            queryInfo.Representations.Add(HelpNames.QUERY_INFO);
            queryInfo.Parameters.Add(parQuery.ToImmutable());
            queryInfo.Documentation.Title = "Help";
            queryInfo.Documentation.Description = "Shows more information and hint for requested command/query.";

            var queryList = Query.CreateBuilder();
            queryList.Key = HelpNames.QUERY_LIST;
            queryList.Representations.Add(HelpNames.QUERY_LIST);
            queryList.Documentation.Title = "Command list";
            queryList.Documentation.Description = "Returns list of all available commands.";

            var queryTree = Query.CreateBuilder();
            queryTree.Key = HelpNames.QUERY_TREE;
            queryTree.Representations.Add(HelpNames.QUERY_TREE);
            queryTree.Parameters.Add(parCommand.ToImmutable());
            queryTree.Options.Add(optionTemplate.ToImmutable());
            queryTree.Documentation.Title = "Command tree";
            queryTree.Documentation.Description = "Returns tree of possible inputs per specific command.";

            command.Queries.AddRange(
                new[]
                {
                    queryInfo.ToImmutable(),
                    queryList.ToImmutable(),
                    queryTree.ToImmutable(),
                });

            return command.ToImmutable();
        }

        public static Query BuildTexo()
        {
            var command = Query.CreateBuilder();

            command.Key = CommandKeys.TEXO;
            command.Representations.AddRange(
                new[] {CommandKeys.TEXO, "textum", "textus", "texui", "texere", "tex", "tx"});
            command.DefaultQueryKey = EnvironmentNames.QUERY_ENVIRONMENT;
            command.Documentation.Title = "Texo management";
            command.Documentation.Description = "Management of the Texo UI environment.";

            command.Queries.Add(CreateEnvironmentQuery());
            command.Queries.Add(CreateHistoryQuery());
            //command.Queries.Add(CreateSettingQuery());

            return command.ToImmutable();
        }

        private static Query CreateEnvironmentQuery()
        {
            var query = Query.CreateBuilder();
            query.Key = EnvironmentNames.QUERY_ENVIRONMENT;
            query.Representations.AddRange(
                new[] { EnvironmentNames.QUERY_ENVIRONMENT, "env", "variable", "var" });
            query.DefaultQueryKey = EnvironmentNames.QUERY_LIST;
            query.Documentation.Title = "Environment variables";
            query.Documentation.Description = "Management of environment variables.";

            var parName = Parameter.CreateBuilder();
            parName.Key = ParameterKeys.NAME;
            parName.ArgumentTemplate = InputRegex.VARIABLE_NAME;
            parName.Documentation.Title = "Variable name";
            parName.Documentation.Description = "Specify variable name.";

            var parValue = Parameter.CreateBuilder();
            parValue.Key = ParameterKeys.VALUE;
            parValue.Documentation.Title = "Variable value";
            parValue.Documentation.Description = "Specify new value of variable.";

            var queryList = Query.CreateBuilder();
            queryList.Key = EnvironmentNames.QUERY_LIST;
            queryList.Representations.AddRange(
                new[] { EnvironmentNames.QUERY_LIST, "show" });

            var queryGet = Query.CreateBuilder();
            queryGet.Key = EnvironmentNames.QUERY_GET;
            queryGet.Representations.Add(EnvironmentNames.QUERY_GET);
            queryGet.Parameters.Add(parName.ToImmutable());

            var querySet = Query.CreateBuilder();
            querySet.Key = EnvironmentNames.QUERY_SET;
            querySet.Representations.Add(EnvironmentNames.QUERY_SET);
            querySet.Parameters.Add(parName.ToImmutable());
            querySet.Parameters.Add(parValue.ToImmutable());

            var queryRemove = Query.CreateBuilder();
            queryRemove.Key = EnvironmentNames.QUERY_REMOVE;
            queryRemove.Representations.AddRange(
                new[] { EnvironmentNames.QUERY_REMOVE, "delete" });
            queryRemove.Parameters.Add(parName.ToImmutable());

            query.Queries.AddRange(
                new[]
                {
                    queryList.ToImmutable(),
                    queryGet.ToImmutable(),
                    querySet.ToImmutable(),
                    queryRemove.ToImmutable()
                });

            return query.ToImmutable();
        }

        private static Query CreateHistoryQuery()
        {
            var query = Query.CreateBuilder();
            query.Key = HistoryNames.QUERY_HISTORY;
            query.Representations.Add(HistoryNames.QUERY_HISTORY);
            query.DefaultQueryKey = HistoryNames.QUERY_LIST;
            query.Documentation.Title = "Command history";
            query.Documentation.Description = "History of commands.";

            var queryList = Query.CreateBuilder();
            queryList.Key = HistoryNames.QUERY_LIST;
            queryList.Representations.AddRange(
                new[] { HistoryNames.QUERY_LIST, "show" });

            var queryClear = Query.CreateBuilder();
            queryClear.Key = HistoryNames.QUERY_CLEAR;
            queryClear.Representations.AddRange(
                new[] { HistoryNames.QUERY_CLEAR, "cls" });

            query.Queries.AddRange(
                new[]
                {
                    queryList.ToImmutable(),
                    queryClear.ToImmutable(),
                });

            return query.ToImmutable();
        }

        private static Query CreateSettingQuery()
        {
            var query = Query.CreateBuilder();
            query.Key = SettingNames.QUERY_SETTING;
            query.Representations.AddRange(
                new[] { SettingNames.QUERY_SETTING, "config" });
            query.Documentation.Title = "Configuration";
            query.Documentation.Description = "Management of the environment configuration.";

            var parCategory = Parameter.CreateBuilder();
            parCategory.Key = SettingNames.PARAMETER_CATEGORY;
            parCategory.ArgumentTemplate = InputRegex.VARIABLE_NAME;
            parCategory.Documentation.Title = "Category";
            parCategory.Documentation.Description = "Specify requested category from settings.";

            var parProperty = Parameter.CreateBuilder();
            parProperty.Key = SettingNames.PARAMETER_PROPERTY;
            parProperty.Documentation.Title = "Property path";
            parProperty.Documentation.Description = "Specify path to property from settings.";

            var parValue = Parameter.CreateBuilder();
            parValue.Key = ParameterKeys.VALUE;
            parValue.Documentation.Title = "Property value";
            parValue.Documentation.Description = "Specify new value of property.";

            var queryList = Query.CreateBuilder();
            queryList.Key = SettingNames.QUERY_LIST;
            queryList.Representations.AddRange(
                new[] { SettingNames.QUERY_LIST, "show" });
            queryList.Parameters.Add(parCategory.ToImmutable());

            var queryGet = Query.CreateBuilder();
            queryGet.Key = EnvironmentNames.QUERY_GET;
            queryGet.Representations.Add(EnvironmentNames.QUERY_GET);
            queryGet.Parameters.Add(parProperty.ToImmutable());

            var querySet = Query.CreateBuilder();
            querySet.Key = EnvironmentNames.QUERY_SET;
            querySet.Representations.Add(EnvironmentNames.QUERY_SET);
            querySet.Parameters.Add(parProperty.ToImmutable());
            querySet.Parameters.Add(parValue.ToImmutable());

            query.Queries.AddRange(
                new[]
                {
                    queryList.ToImmutable(),
                    queryGet.ToImmutable(),
                    querySet.ToImmutable()
                });

            return query.ToImmutable();
        }
    }
}
