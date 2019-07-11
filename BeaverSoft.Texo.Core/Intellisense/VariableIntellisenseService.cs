using System;
using System.Collections.Generic;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Intellisense
{
    public class VariableIntellisenseService : ITokenIntellisenseProvider
    {
        private readonly IEnvironmentService environment;

        public VariableIntellisenseService(IEnvironmentService environment)
        {
            this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public IEnumerable<IItem> Help(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                yield break;
            }

            foreach (var variable in environment.GetVariables())
            {
                if (!variable.Key.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                yield return Item.AsIntellisense(variable.Key, $"${variable.Key}$", "variable", variable.Value ?? "[NULL]");
            }
        }
    }
}
