using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace Commands.CodeBaseSearch.Model
{
    public class SolutionIndex
    {
        private readonly string solutionPath;
        private ISubject root;

        public SolutionIndex(string solutionPath)
        {
            this.solutionPath = solutionPath;
        }

        public async Task PreLoadAsync()
        {
            var solutionSubject = new Subject(Path.GetFileNameWithoutExtension(solutionPath), SubjectTypeEnum.Solution);
            root = solutionSubject;

            MSBuildWorkspace msWorkspace = MSBuildWorkspace.Create();
            Solution solution = await msWorkspace.OpenSolutionAsync(solutionPath);
            var projectListBuilder = ImmutableList<ISubject>.Empty.ToBuilder();

            foreach (Project project in solution.Projects)
            {
                if (string.IsNullOrEmpty(project.FilePath)
                    || (!string.Equals(project.Language, "csharp", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(project.Language, "c#", StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                var projectSubject = new Subject(project.Name, SubjectTypeEnum.Project);
                projectSubject.SetParent(solutionSubject);
                projectListBuilder.Add(projectSubject);

                foreach (Document document in project.Documents)
                {
                    if (document.SourceCodeKind != SourceCodeKind.Regular)
                    {
                        continue;
                    }

                    var documentSubject = new DocumentSubject(document);
                    documentSubject.SetParent(projectSubject);
                }
            }

            solutionSubject.SetChildren(projectListBuilder.ToImmutable());
        }

        public async Task LoadAsync()
        {
            foreach (ISubject project in root.Children)
            {
                foreach (DocumentSubject document in project.Children)
                {
                    await document.LoadAsync();
                }
            }
        }

        public IImmutableList<ISubject> SearchAsync(SearchContext context)
        {
            throw new NotImplementedException();
        }
    }
}
