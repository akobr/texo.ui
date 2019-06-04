using System.Collections.Immutable;
using System.Threading.Tasks;
using Commands.CodeBaseSearch.Model;

namespace Commands.CodeBaseSearch
{
    public class CodeBaseSearchService : ICodeBaseSearchService
    {
        private const string solutionPath = @"C:\Working\textum.ui\BeaverSoft.Texo.sln";
        private readonly IIndex index;

        public CodeBaseSearchService()
        {
            index = new SolutionIndex(new MsBuildSolutionOpenStrategy(solutionPath));
        }

        public Task LoadAsync()
        {
            return index.LoadAsync();
        }

        public Task PreLoadAsync()
        {
            return index.PreLoadAsync();
        }

        public Task<IImmutableList<ISubject>> SearchAsync(SearchContext context)
        {
            return index.SearchAsync(context);
        }
    }
}
