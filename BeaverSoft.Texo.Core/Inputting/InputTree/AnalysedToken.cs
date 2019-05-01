using System;
using System.Text.RegularExpressions;
using BeaverSoft.Texo.Core.Environment;

namespace BeaverSoft.Texo.Core.Inputting.InputTree
{
    internal class AnalysedToken
    {
        private const int FIRST_INDEX = 0;

        private readonly IEnvironmentService environment;
        private readonly string token;
        private readonly Lazy<string> normalisedValue;

        private string value;
        private bool isParameter;
        private bool isExplicitOption;
        private bool isEndOfParameterList;
        private bool isEndOfInput;

        public AnalysedToken(string token, IEnvironmentService environment)
        {
            this.environment = environment;
            this.token = token;
            normalisedValue = new Lazy<string>(BuildNormalisedValue, false);

            Process();
        }

        public string Value => value;

        public string NormalisedValue => normalisedValue.Value;

        public string RawInput => token;

        public bool IsParameter => isParameter;

        public bool IsExplicitOption => isExplicitOption;

        public bool IsEndOfParameterList => isEndOfParameterList;

        public bool IsEndOfInput => isEndOfInput;

        public bool CanBeQuery => !isParameter && !isExplicitOption && !isEndOfParameterList && !isEndOfInput;

        public override string ToString()
        {
            return token;
        }

        private void Process()
        {
            value = token;

            if (IsEndOfInputToken(token))
            {
                isEndOfInput = true;
                return;
            }

            isParameter = !InputRegex.QueryOrOption.IsMatch(token);

            if (isParameter)
            {
                if (IsComplexParameterToken(token))
                {
                    value = value.Substring(1, token.Length - 2);
                }

                value = InputRegex.InlineVariable.Replace(value, ProcessVariable);
            }
            else if (IsExplicitOptionToken(token))
            {
                isExplicitOption = true;
            }
            else if (IsEndOfParameterListToken(token))
            {
                isEndOfParameterList = true;
            }
        }

        private string BuildNormalisedValue()
        {
            if (isParameter)
            {
                return value;
            }

            if (isExplicitOption)
            {
                return value.TrimStart(InputConstants.PARAMETER_INPUT_PREFIX).ToLowerInvariant();
            }

            return value.ToLowerInvariant();
        }

        private string ProcessVariable(Match match)
        {
            isParameter = true;
            string varName = match.Groups[InputRegex.GROUP_NAME].Value;
            return environment.GetVariable(varName);
        }

        private static bool IsComplexParameterToken(string token)
        {
            return token.Length > 1
                   && ((token[FIRST_INDEX] == InputConstants.PRIMARY_STRING_CHARACTER &&
                       token[token.Length - 1] == InputConstants.PRIMARY_STRING_CHARACTER)
                       || (token[FIRST_INDEX] == InputConstants.SECONDARY_STRING_CHARACTER &&
                       token[token.Length - 1] == InputConstants.SECONDARY_STRING_CHARACTER));
        }

        private static bool IsExplicitOptionToken(string token)
        {
            return token.Length > 1
                   && token[FIRST_INDEX] == InputConstants.PARAMETER_INPUT_PREFIX
                   && token != InputConstants.END_OF_PARAMETER_LIST;
        }

        private static bool IsEndOfParameterListToken(string token)
        {
            return token == InputConstants.END_OF_PARAMETER_LIST;
        }

        private static bool IsEndOfInputToken(string token)
        {
            return string.IsNullOrEmpty(token);
        }
    }
}
