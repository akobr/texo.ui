using System;
using System.Collections.Generic;
using BeaverSoft.Texo.Core.Input;

namespace BeaverSoft.Texo.Core.Environment
{
    public class EnvironmentService : IEnvironmentService
    {
        private readonly Dictionary<string, string> variables;

        public EnvironmentService()
        {
            variables = new Dictionary<string, string>();
        }

        public void SetVariable(string variable, string value)
        {
            if (string.IsNullOrEmpty(variable))
            {
                throw new ArgumentNullException(nameof(variable));
            }

            if (!InputRegex.Variable.IsMatch(variable))
            {
                throw new ArgumentException("An invalid variable name.", nameof(variable));
            }

            if (string.IsNullOrEmpty(variable))
            {
                variables.Remove(variable);
            }
            else
            {
                variables[variable] = value;
            }
        }

        public string GetVariable(string variable)
        {
            if (!variables.TryGetValue(variable, out string value))
            {
                return string.Empty;
            }

            return value;
        }
    }
}
