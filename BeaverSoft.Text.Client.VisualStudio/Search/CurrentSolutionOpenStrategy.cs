using System;
using System.Threading.Tasks;
using Commands.CodeBaseSearch;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.ComponentModelHost;

namespace BeaverSoft.Text.Client.VisualStudio.Search
{
    public class CurrentSolutionOpenStrategy : ISolutionOpenStrategy
    {
        private readonly IComponentModel vsComponentModel;


        public CurrentSolutionOpenStrategy(IComponentModel vsComponentModel)
        {
            this.vsComponentModel = vsComponentModel ?? throw new ArgumentNullException(nameof(vsComponentModel));
        }

        public Task<Workspace> OpenAsync()
        {
            var workspace = vsComponentModel.GetService<Microsoft.VisualStudio.LanguageServices.VisualStudioWorkspace>();
            return new VisualStudioSolutionOpenStrategy(workspace).OpenAsync();
        }
    }
}
