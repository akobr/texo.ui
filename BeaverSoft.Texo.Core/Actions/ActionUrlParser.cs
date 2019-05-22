using System;
using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Actions
{
    public class ActionUrlParser : IActionUrlParser
    {
        public IActionContext Parse(string actionUrl)
        {
            Uri uri = new Uri(actionUrl, UriKind.RelativeOrAbsolute);

            if (!uri.IsAbsoluteUri
                || !string.Equals(uri.Scheme, ActionConstants.ACTION_SCHEMA, StringComparison.OrdinalIgnoreCase))
            {
                return new ActionContext()
                {
                    Name = ActionNames.URI,
                    Arguments = new Dictionary<string, string>()
                    {
                        { ActionParameters.ACTION_NAME, ActionNames.URI },
                        { ActionParameters.URI, actionUrl }
                    }
                };
            }

            string actionName = uri.Host;
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments[ActionParameters.ACTION_NAME] = actionName;

            ParseSegments(uri.Segments, arguments);
            ParseQuery(uri.Query, arguments);
            ParseFragment(uri.Fragment, arguments);

            return new ActionContext()
            {
                Name = actionName,
                Arguments = arguments
            };
        }

        private static void ParseSegments(IEnumerable<string> segments, IDictionary<string, string> arguments)
        {
            foreach (string segment in segments)
            {
                if (segment == "/")
                {
                    continue;
                }

                arguments[segment] = segment;
            }
        }

        private static void ParseQuery(string query, IDictionary<string, string> arguments)
        {
            if (string.IsNullOrEmpty(query))
            {
                return;
            }

            string[] pairs = query.Split(new[] {'?', '&'}, StringSplitOptions.RemoveEmptyEntries);

            foreach (string pair in pairs)
            {
                string[] values = pair.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);

                if (values.Length != 2)
                {
                    continue;
                }

                arguments[values[0]] = Uri.UnescapeDataString(values[1]);
            }
        }

        private static void ParseFragment(string fragment, IDictionary<string, string> arguments)
        {
            if (string.IsNullOrEmpty(fragment))
            {
                return;
            }

            arguments[ActionParameters.ACTION_ARGS] = fragment;
        }
    }
}