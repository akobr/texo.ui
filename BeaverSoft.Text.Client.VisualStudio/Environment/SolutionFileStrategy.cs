using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Text.Client.VisualStudio.Core.Environment;
using System.IO;

namespace BeaverSoft.Text.Client.VisualStudio.Environment
{
    public class SolutionFileStrategy : IVariableStrategy
    {
        private readonly IEnvironmentService texoEnvironment;

        public SolutionFileStrategy(IEnvironmentService texoEnvironment)
        {
            this.texoEnvironment = texoEnvironment;
        }

        public bool CanBeRemoved(string currentValue)
        {
            return false;
        }

        public bool IsValueValid(string newValue, string currentValue)
        {
            return File.Exists(newValue) && string.Equals(Path.GetExtension(newValue), ".sln");
        }

        public void OnValueChange(string value)
        {
            texoEnvironment.SetVariable(VsVariableNames.SOLUTION_DIRECTORY, Path.GetDirectoryName(value));
        }
    }
}
