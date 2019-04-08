using System;
using System.Collections.Generic;
using System.Text;

namespace BeaverSoft.Texo.Core.Actions
{
    public static class ActionBuilder
    {
        public static string PathUri(string fullPath)
        {
            return BuildActionUri(ActionNames.PATH, ActionParameters.PATH, fullPath);
        }

        public static string PathOpenUri(string fileFullPath)
        {
            return BuildActionUri(ActionNames.PATH_OPEN, ActionParameters.PATH, fileFullPath);
        }

        public static string CommandUri(string input)
        {
            return BuildActionUri(ActionNames.COMMAND, ActionParameters.STATEMENT, input);
        }

        public static string CommandRunUri(string input)
        {
            return BuildActionUri(ActionNames.COMMAND_RUN, ActionParameters.STATEMENT, input);
        }

        public static string InputUri(string input)
        {
            return BuildActionUri(ActionNames.INPUT, ActionParameters.INPUT, input);
        }

        public static string InputSetUri(string input)
        {
            return BuildActionUri(ActionNames.INPUT_SET, ActionParameters.INPUT, input);
        }

        public static string InputAddUri(string append)
        {
            return BuildActionUri(ActionNames.INPUT_ADD, ActionParameters.INPUT, append);
        }

        private static string BuildActionUri(string actionName, IDictionary<string, string> arguments)
        {
            StringBuilder builder = new StringBuilder(ActionConstants.ACTION_SCHEMA + Uri.SchemeDelimiter);
            builder.Append(actionName ?? throw new ArgumentNullException(nameof(actionName)));
            
            if (arguments == null || arguments.Count < 1)
            {
                return builder.ToString();
            }

            builder.Append('?');

            foreach (var argument in arguments)
            {
                builder.AppendFormat("{0}={1}", argument.Key, Uri.EscapeDataString(argument.Value));
                builder.Append('&');
            }

            builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }

        private static string BuildActionUri(string actionName, string argumentName, string argumentValue)
        {
            return BuildActionUri(actionName, new Dictionary<string, string> { { argumentName, argumentValue } });
        }

        private static string BuildActionUri(string actionName)
        {
            return BuildActionUri(actionName, null);
        }
    }
}
