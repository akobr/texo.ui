using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Extensibility.Attributes;

namespace BeaverSoft.Texo.Core.Extensibility.Loader
{
    public class LoadedCommand
    {
        private Type commandInterfaceType = typeof(ICommand);

        public LoadedCommand(Type commandType)
        {
            CommandType = commandType;
            ProcessType();
        }

        public Type CommandType { get; }

        public string Key { get; private set; }

        public bool HasExecuteMethod { get; private set; }

        public IDictionary<string, ILoadedQuery> Queries { get; set; }

        public IDictionary<string, ILoadedDocumentation> Documentation { get; set; }

        public void AddQueryAsClass(Type queryType)
        {

        }

        private void ProcessType()
        {
            CommandAttribute attCommand = CommandType.GetCustomAttribute<CommandAttribute>(false);

            Key = attCommand.CommandKey;
            HasExecuteMethod = commandInterfaceType.IsAssignableFrom(CommandType)
                || IsCustomCommand(CommandType);

            foreach (MethodInfo method in CommandType.GetMethods(BindingFlags.Public))
            {
                QueryAttribute attQuery = method.GetCustomAttribute<QueryAttribute>(false);
            }
            
        }

        private static void LoadOptions(MemberInfo member, Query.Builder queryBuilder)
        {
            foreach (OptionAttribute option in member.GetCustomAttributes<OptionAttribute>(false))
            {
                Option.Builder optionBuilder = Option.CreateBuilder();
                optionBuilder.Key = option.OptionKey;
            }
        }

        private static string[] ReadRepresentations(string representations)
        {
            return representations.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }


        private static bool IsCustomCommand(Type commandType)
        {
            MethodInfo executionMethod = commandType.GetMethod(nameof(ICommand.Execute), BindingFlags.Public);
            ParameterInfo[] parameters = executionMethod?.GetParameters();

            return executionMethod != null
                && parameters.Length == 1
                && parameters[0].ParameterType == typeof(CommandContext);
        }
    }
}
