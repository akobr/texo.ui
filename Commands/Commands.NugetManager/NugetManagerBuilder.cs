using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Input;

namespace BeaverSoft.Texo.Commands.NugetManager
{
    public static class NugetManagerBuilder
    {
        public static Query BuildCommand()
        {
            var command = Query.CreateBuilder();

            command.Key = NugetManagerQueries.NUGET;
            command.Representations.AddRange(
                new[] { NugetManagerQueries.NUGET, "nmanager", "nm", "nuget" });
            command.DefaultQueryKey = StageQueries.STAGE;

            command.Queries.Add(CreateStageQuery());

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

            var queryAdd = Query.CreateBuilder();
            queryAdd.Key = StageQueries.ADD;
            queryAdd.Representations.Add(StageQueries.ADD);
            queryAdd.Parameters.Add(parPath.ToImmutable());

            var queryRemove = Query.CreateBuilder();
            queryRemove.Key = StageQueries.REMOVE;
            queryRemove.Representations.AddRange(
                new[] { StageQueries.REMOVE, "rm" });
            queryRemove.Parameters.Add(parPath.ToImmutable());

            query.Queries.AddRange(
                new[]
                {
                    queryStatus.ToImmutable(),
                    queryAdd.ToImmutable(),
                    queryRemove.ToImmutable(),
                });

            return query.ToImmutable();
        }
    }
}
