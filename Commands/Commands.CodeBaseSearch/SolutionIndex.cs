using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Commands.CodeBaseSearch.Model.KeywordBuilders;
using Commands.CodeBaseSearch.Model.Subjects;
using Microsoft.CodeAnalysis;

namespace Commands.CodeBaseSearch.Model
{
    public class SolutionIndex : IIndex
    {
        private readonly ISolutionOpenStrategy openStrategy;

        private IImmutableDictionary<string, Category> categories;
        private Category projectsCategory;
        private Category filesCategory;
        private Category typesCategory;
        private Category membersCategory;

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

            BuildCategories();
            root = new SolutionSubject(Path.GetFileNameWithoutExtension(solution.FilePath));

            foreach (Project project in solution.Projects)
            {
                if (string.IsNullOrEmpty(project.FilePath)
                    || (!string.Equals(project.Language, "csharp", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(project.Language, "c#", StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                var projectSubject = new ProjectSubject(project);
                projectSubject.SetParent(root);

                foreach (Document document in project.Documents)
                {
                    if (document.SourceCodeKind != SourceCodeKind.Regular)
                    {
                        continue;
                    }

                    var documentSubject = new DocumentSubject(document);
                    documentSubject.SetParent(projectSubject);
                    filesCategory.AddSubject(documentSubject);
                    projectSubject.SetChildren(projectSubject.Children.Add(documentSubject));
                }

                projectsCategory.AddSubject(projectSubject);
                root.SetChildren(root.Children.Add(projectSubject));
            }
        }

        public async Task LoadAsync()
        {
            foreach (ISubject project in root.Children)
            {
                foreach (DocumentSubject document in project.Children)
                {
                    if (document.IsLoaded)
                    {
                        continue;
                    }

                    await document.LoadAsync();
                    CategoriseDocument(document);
                    await Task.Delay(7); // TODO: check CPU usage
                }
            }
        }

        // TODO: refactor this mess
        public Task<IImmutableList<ISubject>> SearchAsync(SearchContext context)
        {         
            var searchKeywords = new NameKeywordBuilder(context.SearchTerm).Build();
            Dictionary<ISubject, int> hitMap = new Dictionary<ISubject, int>();
            
            foreach (Category category in GetCategoriesToSearch(context))
            {
                foreach (string keyword in searchKeywords)
                {
                    if (category.Map.TryGetValue(keyword, out var subjects))
                    {
                        foreach (ISubject subject in subjects.Take(50))
                        {
                            hitMap.TryGetValue(subject, out int hit);
                            hitMap[subject] = ++hit;
                        }
                    }

                    if (hitMap.Count > 200)
                    {
                        break;
                    }
                }

                if (hitMap.Count > 200)
                {
                    break;
                }
            }

            var builder = ImmutableList<ISubject>.Empty.ToBuilder();
            List<KeyValuePair<ISubject, int>> list = new List<KeyValuePair<ISubject, int>>(hitMap);
            list.Sort((x, y) => y.Value.CompareTo(x.Value));
            builder.AddRange(list.Select(item => item.Key));
            return Task.FromResult<IImmutableList<ISubject>>(builder.ToImmutable());
        }

        public IEnumerable<ICategory> GetCategories()
        {
            yield return typesCategory;
            yield return membersCategory;
            yield return filesCategory;
            yield return projectsCategory;

            foreach (ICategory category in categories.Values)
            {
                yield return category;
            }
        }

        private void BuildCategories()
        {
            projectsCategory = new Category("projects", 'p');
            filesCategory = new Category("files", 'f');
            typesCategory = new Category("types", 't');
            membersCategory = new Category("members", 'm');

            var userCategories = ImmutableDictionary<string, Category>.Empty.ToBuilder();

            foreach (IRule rule in RulesBuilder.Build())
            {
                Category category = new Category(rule.CategoryName, rule.CategoryCharacter);
                category.LinkCondition(rule.Condition);
                category.LinkGrouping(rule.Grouping);
                userCategories.Add(category.Name, category);
            }

            categories = userCategories.ToImmutable();
        }

        private IEnumerable<Category> GetCategoriesToSearch(SearchContext context)
        {
            if (context.Categories.Count < 1
                || context.Categories.Contains("types"))
            {
                yield return typesCategory;
            }

            if (context.Categories.Contains("members"))
            {
                yield return membersCategory;
            }

            if (context.Categories.Contains("files"))
            {
                yield return filesCategory;
            }

            if (context.Categories.Contains("projects"))
            {
                yield return projectsCategory;
            }

            foreach (Category category in categories.Values)
            {
                if (context.Categories.Contains(category.Name)
                    || context.Categories.Contains(category.Character.ToString()))
                {
                    yield return category;
                }
            }
        }

        private void CategoriseDocument(DocumentSubject document)
        {
            if (!document.IsLoaded)
            {
                return;
            }

            foreach (ISubject type in document.Children)
            {
                typesCategory.AddSubject(type);

                foreach (Category category in categories.Values)
                {
                    if (category.Condition.IsMet(type))
                    {
                        category.AddSubject(type);
                    }
                }

                foreach (ISubject member in type.Children)
                {
                    membersCategory.AddSubject(member);
                }
            }
        }
    }
}
