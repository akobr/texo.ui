using System;
using System.Collections.Immutable;
using StrongBeaver.Core;

namespace BeaverSoft.Texo.Core.Environment
{
    public interface IEnvironmentService : IInitialisable, IDisposable
    {
        int Count { get; }

        void SetVariable(string variable, string value);

        string GetVariable(string variable);

        string GetVariable(string variable, string defaultValue);

        IImmutableDictionary<string, string> GetVariables();
    }
}