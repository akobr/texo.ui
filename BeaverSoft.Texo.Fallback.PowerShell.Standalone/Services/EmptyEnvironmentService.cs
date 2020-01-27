using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Environment;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.Services
{
    public class EmptyEnvironmentService : IEnvironmentService
    {
        public int Count => 0;

        public void Initialise() { }

        public void Dispose() { }

        public string GetVariable(string variable) => null;

        public string GetVariable(string variable, string defaultValue) => defaultValue;

        public IImmutableDictionary<string, string> GetVariables() => ImmutableDictionary<string, string>.Empty;

        public void RegisterVariableStrategy(string variable, IVariableStrategy strategy) { }

        public void SetVariable(string variable, string value) { }
    }
}
