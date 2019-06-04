using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.LanguageServices;

namespace Commands.CodeBaseSearch
{
    public class VisualStudioSolutionOpenStrategy : ISolutionOpenStrategy
    {
        private readonly VisualStudioWorkspace workspace;

        public VisualStudioSolutionOpenStrategy(VisualStudioWorkspace workspace)
        {
            this.workspace = workspace;
        }

        public Task<Workspace> OpenAsync()
        {
            return workspace;
        }
    }
}
