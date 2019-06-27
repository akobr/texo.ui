using BeaverSoft.Texo.Commands.NugetManager.Manage;
using BeaverSoft.Texo.Commands.NugetManager.Stage;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Inputting;

namespace BeaverSoft.Texo.Commands.NugetManager
{
    public static class NugetManagerBuilder
    {
        public static Query BuildCommand()
        {
            var command = Query.CreateBuilder();

            command.Key = NugetManagerQueries.NUGET;
            command.Representations.AddRange(
                new[] { NugetManagerQueries.NUGET, "nman", "nm", "nuget" });
            command.DefaultQueryKey = StageQueries.STAGE;
            command.Documentation.Title = "Nuget manager";
            command.Documentation.Description = "Naive command line Nuget package manager.";

            command.Queries.Add(CreateStageQuery());
            command.Queries.Add(CreateProjectQuery());
            command.Queries.Add(CreatePackageQuery());

            return command.ToImmutable();
        }

        private static Query CreateStageQuery()
        {
            var query = Query.CreateBuilder();
            query.Key = StageQueries.STAGE;
            query.Representations.Add(StageQueries.STAGE);
            query.DefaultQueryKey = StageQueries.STATUS;

            var parPath = Parameter.CreateBuilder();
            parPath.Key = ParameterKeys.PATH;
            parPath.ArgumentTemplate = InputRegex.WILDCARD_PATH;
            parPath.IsRepeatable = true;
            parPath.Documentation.Title = "Solution/Project(s) path";
            parPath.Documentation.Description = "Specify path(s), wildcards can be used: *, ? or **.";

            var queryStatus = Query.CreateBuilder();
            queryStatus.Key = StageQueries.STATUS;
            queryStatus.Representations.AddRange(
                new[] { StageQueries.STATUS, "show" });

            var optionProjects = Option.CreateBuilder();
            optionProjects.Key = StageOptions.PROJECTS;
            optionProjects.Representations.AddRange(
                new[] { StageOptions.PROJECTS, "p" });
            queryStatus.Options.Add(optionProjects.ToImmutable());

            var optionConfigs = Option.CreateBuilder();
            optionConfigs.Key = StageOptions.CONFIGS;
            optionConfigs.Representations.AddRange(
                new[] { StageOptions.CONFIGS, "c" });
            queryStatus.Options.Add(optionConfigs.ToImmutable());

            var optionSources = Option.CreateBuilder();
            optionSources.Key = StageOptions.SOURCES;
            optionSources.Representations.AddRange(
                new[] { StageOptions.SOURCES, "s" });
            queryStatus.Options.Add(optionSources.ToImmutable());

            var queryFetch = Query.CreateBuilder();
            queryFetch.Key = StageQueries.FETCH;
            queryFetch.Representations.AddRange(
                new[] { StageQueries.FETCH, "update" });

            var queryAdd = Query.CreateBuilder();
            queryAdd.Key = StageQueries.ADD;
            queryAdd.Representations.Add(StageQueries.ADD);
            queryAdd.Parameters.Add(parPath.ToImmutable());

            var queryRemove = Query.CreateBuilder();
            queryRemove.Key = StageQueries.REMOVE;
            queryRemove.Representations.AddRange(
                new[] { StageQueries.REMOVE, "rm" });
            queryRemove.Parameters.Add(parPath.ToImmutable());

            var queryClear = Query.CreateBuilder();
            queryClear.Key = StageQueries.CLEAR;
            queryClear.Representations.AddRange(
                new[] { StageQueries.CLEAR, "clean", "cls" });

            query.Queries.AddRange(
                new[]
                {
                    queryStatus.ToImmutable(),
                    queryFetch.ToImmutable(),
                    queryAdd.ToImmutable(),
                    queryRemove.ToImmutable(),
                    queryClear.ToImmutable(),
                });

            return query.ToImmutable();
        }

        private static Query CreatePackageQuery()
        {
            var query = Query.CreateBuilder();
            query.Key = ManageQueries.PACKAGE;
            query.Representations.Add(ManageQueries.PACKAGE);

            var parTerm = Parameter.CreateBuilder();
            parTerm.Key = NugetManagerParameters.SEARCH_TERM;
            parTerm.ArgumentTemplate = @"^[A-Za-z0-9._\-]*$";
            parTerm.Documentation.Title = "Package id or a search term";
            parTerm.Documentation.Description = "Specify a package id or a search term.";
            query.Parameters.Add(parTerm.ToImmutable());

            return query.ToImmutable();
        }

        private static Query CreateProjectQuery()
        {
            var query = Query.CreateBuilder();
            query.Key = ManageQueries.PROJECT;
            query.Representations.Add(ManageQueries.PROJECT);

            var parTerm = Parameter.CreateBuilder();
            parTerm.Key = NugetManagerParameters.SEARCH_TERM;
            parTerm.ArgumentTemplate = @"^[A-Za-z0-9._\-]*$";
            parTerm.Documentation.Title = "Project name or a search term";
            parTerm.Documentation.Description = "Specify a project name or a search term.";
            query.Parameters.Add(parTerm.ToImmutable());

            return query.ToImmutable();
        }
    }
}
