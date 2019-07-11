using System;
using System.Collections.Generic;
using System.Linq;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Intellisense
{
    public class CommandListIntellisenseProvider : ISynchronousIntellisenseProvider
    {
        private TextumConfiguration configuration;

        public void SetConfiguration(TextumConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IEnumerable<IItem> GetHelp(Input input)
        {
            string token = input.ParsedInput.IsEmpty() ? string.Empty : input.ParsedInput.Tokens[0];

            foreach (Query query in configuration.Runtime.Commands.OrderBy(cmd => cmd.Key))
            {
                if (!query.Representations.Any(r => r.StartsWith(token, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                yield return Item.AsIntellisense(query.GetMainRepresentation(), "command", query.Documentation.Description);
            }
        }
    }
}
