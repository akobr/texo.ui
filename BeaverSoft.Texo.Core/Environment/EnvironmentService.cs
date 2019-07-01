using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Inputting;
using StrongBeaver.Core.Messaging;
using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.Environment
{
    public class EnvironmentService : IEnvironmentService, IMessageBusService<ISettingUpdatedMessage>, IMessageBusService<ISetVariableMessage>
    {
        private readonly IServiceMessageBus messageBus;
        private readonly Dictionary<string, string> variables;
        private readonly Dictionary<string, IVariableStrategy> strategies;

        public EnvironmentService(IServiceMessageBus messageBus)
        {
            this.messageBus = messageBus;
            variables = new Dictionary<string, string>();
            strategies = new Dictionary<string, IVariableStrategy>();

            CreateVariableStrategies();
        }

        public void CreateVariableStrategies()
        {
            strategies[VariableNames.CURRENT_DIRECTORY] = new CurrentDirectoryStrategy();
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
            return GetVariable(variable, string.Empty);
        }

        public string GetVariable(string variable, string defaultValue)
        {
            if (!variables.TryGetValue(variable, out string value))
            {
                return defaultValue;
            }

            return value;
        }

        public IImmutableDictionary<string, string> GetVariables()
        {
            var builder = ImmutableDictionary<string, string>.Empty.ToBuilder();
            builder.AddRange(variables);
            return builder.ToImmutable();
        }

        public void RegisterVariableStrategy(string variable, IVariableStrategy strategy)
        {
            if (string.IsNullOrEmpty(variable))
            {
                throw new ArgumentNullException(nameof(variable));
            }

            strategies[variable] = strategy ?? throw new ArgumentNullException(nameof(strategy));
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

            if (previousValue == value)
            {
                return;
            }

            strategies.TryGetValue(variable, out IVariableStrategy strategy);

            if (string.IsNullOrEmpty(value))
            {
                if (!strategy?.CanBeRemoved(previousValue) ?? false)
                {
                    return;
                }

                variables.Remove(variable);
            }
            else
            {
                if (!strategy?.IsValueValid(value, previousValue) ?? false)
                {
                    return;
                }

                variables[variable] = value;
            }

            strategy?.OnValueChange(value);
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

        void IMessageBusRecipient<ISetVariableMessage>.ProcessMessage(ISetVariableMessage message)
        {
            SetVariable(message.Name, message.Value);
        }
    }
}