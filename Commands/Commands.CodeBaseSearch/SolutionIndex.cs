using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Commands.CodeBaseSearch.Model
{
    public class SolutionIndex : IIndex
    {
        private readonly ISolutionOpenStrategy openStrategy;
        private Workspace workspace;
        private Subject root;

        public SolutionIndex(ISolutionOpenStrategy openStrategy)
        {
            this.openStrategy = openStrategy;
        }

        public async Task PreLoadAsync()
        {
            workspace = await openStrategy.OpenAsync();
            Solution solution = workspace.CurrentSolution;

            if (solution?.FilePath == null)
            {
                return;
            }

            root = new Subject(Path.GetFileNameWithoutExtension(solution.FilePath), SubjectTypeEnum.Solution);
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
                projectSubject.SetParent(root);
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

            root.SetChildren(projectListBuilder.ToImmutable());
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

        public Task<IImmutableList<ISubject>> SearchAsync(SearchContext context)
        {
            var builder = ImmutableList<ISubject>.Empty.ToBuilder();

            foreach (ISubject child in root.Children)
            {
                SearchSubject(child, context, builder);
            }

            return Task.FromResult<IImmutableList<ISubject>>(builder.ToImmutable());
        }

        private void SearchSubject(ISubject subject, SearchContext context, ImmutableList<ISubject>.Builder builder)
        {
            if (subject.Keywords.Contains(context.SearchTerm))
            {
                builder.Add(subject);
            }

            foreach (ISubject child in subject.Children)
            {
                SearchSubject(child, context, builder);
            }
        }
    }
}
