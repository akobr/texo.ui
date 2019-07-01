using BeaverSoft.Texo.Core.Configuration;

namespace Commands.CodeBaseSearch
{
    public static class CodeBaseSearchBuilder
    {
        public static Query BuildCommand()
        {
            var command = Query.CreateBuilder();
            command.Key = CodeBaseSearchConstants.COMMAND_NAME;
            command.DefaultQueryKey = CodeBaseSearchConstants.QUERY_SEARCH;
            command.Representations.AddRange(
                new[] { CodeBaseSearchConstants.COMMAND_NAME, "code-search", "search", "s" });
            command.Documentation.Title = "Code-base search";
            command.Documentation.Description = "Cobe-base search with context and rule definitions.";

            var init = Query.CreateBuilder();
            init.Key = CodeBaseSearchConstants.QUERY_INIT;
            init.Representations.AddRange(
                new[] { CodeBaseSearchConstants.QUERY_INIT, "load", "build" });
            init.Documentation.Title = "code-base-init";
            init.Documentation.Description = "Initialises the search index.";

            var search = Query.CreateBuilder();
            search.Key = CodeBaseSearchConstants.QUERY_SEARCH;
            search.Representations.AddRange(
                new[] { CodeBaseSearchConstants.QUERY_SEARCH });
            search.Documentation.Title = "code-base-search";
            search.Documentation.Description = "Process a search in code-base.";

            var parTerm = Parameter.CreateBuilder();
            parTerm.Key = CodeBaseSearchConstants.PARAMETER_TERM;
            parTerm.ArgumentTemplate = ".+";
            parTerm.IsRepeatable = true;
            parTerm.Documentation.Title = "search-term";
            parTerm.Documentation.Description = "Term of search.";
            search.Parameters.Add(parTerm.ToImmutable());

            var categories = Query.CreateBuilder();
            categories.Key = CodeBaseSearchConstants.QUERY_CATEGORIES;
            categories.Representations.AddRange(
                new[] { CodeBaseSearchConstants.QUERY_CATEGORIES, "category", "cat" });
            categories.Documentation.Title = "code-base-categories";
            categories.Documentation.Description = "Shows a list of all registered categories.";

            command.Queries.AddRange(
                new[]
                {
                    init.ToImmutable(),
                    search.ToImmutable(),
                    categories.ToImmutable(),
                });

            return command.ToImmutable();
        }
    }
}
