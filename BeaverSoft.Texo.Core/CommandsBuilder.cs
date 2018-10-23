using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Input;

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

        public static Query BuildHelp()
        {
            var command = Query.CreateBuilder();

            var parameter = Parameter.CreateBuilder();
            parameter.Key = ParameterKeys.ITEM;
            parameter.ArgumentTemplate = InputRegex.QUERY_OR_OPTION;
            parameter.IsOptional = true;
            parameter.IsRepeatable = true;
            parameter.Documentation.Title = "Query";
            parameter.Documentation.Description = "Specify command / query / option or parameter name about which you want to know more.";

            command.Key = CommandKeys.HELP;
            command.Representations.AddRange(
                new[] { CommandKeys.HELP, "advice", "aid" });
            command.Parameters.Add(parameter.ToImmutable());
            command.Documentation.Title = "Help";
            command.Documentation.Description = "Shows more information and hint for requested query.";

            return command.ToImmutable();
        }

        public static Query BuildTexo()
        {
            var command = Query.CreateBuilder();

            command.Key = CommandKeys.TEXO;
            command.Representations.AddRange(
                new[] {CommandKeys.TEXO, "textum", "textus", "texui", "texere", "tex", "tx"});

            command.Queries.Add(CreateEnvironmentQuery());
            command.Queries.Add(CreateSettingQuery());

            return command.ToImmutable();
        }

        private static Query CreateEnvironmentQuery()
        {
            var query = Query.CreateBuilder();
            query.Key = EnvironmentNames.QUERY_ENVIRONMENT;
            query.Representations.AddRange(
                new[] { EnvironmentNames.QUERY_ENVIRONMENT, "env", "variable", "var" });
            query.DefaultQueryKey = EnvironmentNames.QUERY_LIST;

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

        private static Query CreateSettingQuery()
        {
            var query = Query.CreateBuilder();
            query.Key = SettingNames.QUERY_SETTING;
            query.Representations.AddRange(
                new[] { SettingNames.QUERY_SETTING, "config" });

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
