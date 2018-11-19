using System;
using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Configuration;
using StrongBeaver.Core;
using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.Environment
{
    public interface IEnvironmentService :
        IInitialisable,
        IDisposable,
        IMessageBusService<ISettingUpdatedMessage>
    {
        int Count { get; }

        void SetVariable(string variable, string value);

        string GetVariable(string variable);

        IImmutableDictionary<string, string> GetVariables();
    }
}