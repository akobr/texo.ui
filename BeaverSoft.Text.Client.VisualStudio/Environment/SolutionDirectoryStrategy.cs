using System.IO;
using BeaverSoft.Texo.Core.Environment;

namespace BeaverSoft.Text.Client.VisualStudio.Environment
{
    public class SolutionDirectoryStrategy : IVariableStrategy
    {
        private readonly IEnvironmentService texoEnvironment;

        public SolutionDirectoryStrategy(IEnvironmentService texoEnvironment)
        {
            this.texoEnvironment = texoEnvironment;
        }

        public bool CanBeRemoved(string currentValue)
        {
            return false;
        }

        public bool IsValueValid(string newValue, string currentValue)
        {
            return Directory.Exists(newValue);
        }

        public void OnValueChange(string value)
        {
            texoEnvironment.SetVariable(VariableNames.CURRENT_DIRECTORY, value);
        }
    }
}
