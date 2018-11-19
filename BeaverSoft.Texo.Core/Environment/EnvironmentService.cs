using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Input;
using StrongBeaver.Core.Messaging;
using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.Environment
{
    public class EnvironmentService : IEnvironmentService
    {
        private readonly IServiceMessageBus messageBus;
        private readonly Dictionary<string, string> variables;

        public EnvironmentService(IServiceMessageBus messageBus)
        {
            this.messageBus = messageBus;
            variables = new Dictionary<string, string>();
        }

        public void Initialise()
        {
            // TODO: load saved
        }

        public int Count => variables.Count;


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

            UpdateVariable(variable, value);
        }

        public string GetVariable(string variable)
        {
            if (!variables.TryGetValue(variable, out string value))
            {
                return string.Empty;
            }

            return value;
        }

        public IImmutableDictionary<string, string> GetVariables()
        {
            var builder = ImmutableDictionary<string, string>.Empty.ToBuilder();
            builder.AddRange(variables);
            return builder.ToImmutable();
        }

        public void Dispose()
        {
            // TODO: save
        }

        private void UpdateVariable(string variable, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                value = string.Empty;
            }

            if (!variables.TryGetValue(variable, out string previousValue)
                && string.IsNullOrEmpty(value))
            {
                return;
            }

            if (string.IsNullOrEmpty(value))
            {
                if (variable == VariableNames.CURRENT_DIRECTORY)
                {
                    throw new ArgumentException("Variable " + VariableNames.CURRENT_DIRECTORY + " can't be removed.", nameof(variable));
                }

                variables.Remove(variable);
            }
            else
            {
                variables[variable] = value;
            }

            messageBus?.Send(new VariableUpdatedMessage(variable, value, previousValue)
            {
                Sender = this
            });
        }

        void IMessageBusRecipient<ISettingUpdatedMessage>.ProcessMessage(ISettingUpdatedMessage message)
        {
            foreach (var variable in message.Configuration.Environment.Variables)
            {
                if (!variables.ContainsKey(variable.Key))
                {
                    SetVariable(variable.Key, variable.Value);
                }
            }
        }
    }
}