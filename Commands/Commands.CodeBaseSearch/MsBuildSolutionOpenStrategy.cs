using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace Commands.CodeBaseSearch
{
    public class MsBuildSolutionOpenStrategy : ISolutionOpenStrategy
    {
        private readonly string solutionPath;
        private MSBuildWorkspace msWorkspace;

        public MsBuildSolutionOpenStrategy(string solutionPath)
        {
            this.solutionPath = solutionPath;
        }

        public async Task<Workspace> OpenAsync()
        {
            if (msWorkspace != null)
            {
                return msWorkspace;
            }

            msWorkspace = MSBuildWorkspace.Create();
            await msWorkspace.OpenSolutionAsync(solutionPath);
            return msWorkspace;
        }
    }
}
