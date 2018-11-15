using BeaverSoft.Texo.Commands.FileManager.Operations;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Commands.FileManager.Stash;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Input;

namespace BeaverSoft.Texo.Commands.FileManager
{
    public static class FileManagerBuilder
    {
        public static Query BuildCommand()
        {
            var command = Query.CreateBuilder();

            command.Key = FileManagerQueries.FILE_MANAGER;
            command.Representations.AddRange(
                new[] { FileManagerQueries.FILE_MANAGER, "fmanager", "fm" });
            command.DefaultQueryKey = StageQueries.STAGE;

            command.Queries.Add(CreateStageQuery());
            command.Queries.Add(CreateStashQuery());
            command.Queries.Add(CreateApplyQuery());

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
            parPath.Documentation.Title = "Complex path";
            parPath.Documentation.Description = "Specify path(s), wildcards can be used: *, ? or **.";

            var parLobbyPath = Parameter.CreateBuilder();
            parLobbyPath.Key = FileManagerParameters.SIMPLE_PATH;
            parLobbyPath.ArgumentTemplate = InputRegex.PATH;
            parLobbyPath.Documentation.Title = "Lobby path";
            parLobbyPath.Documentation.Description = "Specify simple path to lobby directory.";

            var queryStatus = Query.CreateBuilder();
            queryStatus.Key = StageQueries.STATUS;
            queryStatus.Representations.AddRange(
                new[] { StageQueries.STATUS, "show" });

            var queryList = Query.CreateBuilder();
            queryList.Key = StageQueries.LIST;
            queryList.Representations.AddRange(
                new[] { StageQueries.LIST, "dir", "ls" });
            parPath.IsOptional = true;
            queryList.Parameters.Add(parPath.ToImmutable());

            var queryAdd = Query.CreateBuilder();
            queryAdd.Key = StageQueries.ADD;
            queryAdd.Representations.Add(StageQueries.ADD);
            parPath.IsOptional = false;
            queryAdd.Parameters.Add(parPath.ToImmutable());

            var queryRemove = Query.CreateBuilder();
            queryRemove.Key = StageQueries.REMOVE;
            queryRemove.Representations.AddRange(
                new[] { StageQueries.REMOVE, "rm" });
            queryRemove.Parameters.Add(parPath.ToImmutable());

            var queryLobby = Query.CreateBuilder();
            queryLobby.Key = StageQueries.LOBBY;
            queryLobby.Representations.Add(StageQueries.LOBBY);
            queryLobby.Parameters.Add(parLobbyPath.ToImmutable());

            var queryRemoveLobby = Query.CreateBuilder();
            queryRemoveLobby.Key = StageQueries.REMOVE_LOBBY;
            queryRemoveLobby.Representations.AddRange(
                new[] { StageQueries.REMOVE_LOBBY, "rm-lobby", "rml" });
            queryRemoveLobby.Parameters.Add(parLobbyPath.ToImmutable());

            query.Queries.AddRange(
                new[]
                {
                    queryStatus.ToImmutable(),
                    queryList.ToImmutable(),
                    queryAdd.ToImmutable(),
                    queryRemove.ToImmutable(),
                    queryLobby.ToImmutable(),
                    queryRemoveLobby.ToImmutable()
                });

            return query.ToImmutable();
        }

        private static Query CreateStashQuery()
        {
            var query = Query.CreateBuilder();
            query.Key = StashQueries.STASH;
            query.Representations.Add(StashQueries.STASH);
            query.DefaultQueryKey = StashQueries.POP;

            var parIdentifier = Parameter.CreateBuilder();
            parIdentifier.Key = StashParameters.IDENTIFIER;
            parIdentifier.ArgumentTemplate = InputRegex.VARIABLE_NAME;
            parIdentifier.IsOptional = true;
            parIdentifier.Documentation.Title = "Stash identifier";
            parIdentifier.Documentation.Description = "Specify stash identifier, index or name.";

            var parName = Parameter.CreateBuilder();
            parName.Key = ParameterKeys.NAME;
            parName.ArgumentTemplate = InputRegex.VARIABLE_NAME;
            parName.IsOptional = true;
            parName.Documentation.Title = "Stash name";
            parName.Documentation.Description = "Specify stash name.";

            var queryList = Query.CreateBuilder();
            queryList.Key = StashQueries.LIST;
            queryList.Representations.AddRange(
                new[] { StashQueries.LIST, "show" });

            var queryApply = Query.CreateBuilder();
            queryApply.Key = StashQueries.APPLY;
            queryApply.Representations.Add(StashQueries.APPLY);
            queryApply.Parameters.Add(parIdentifier.ToImmutable());

            var queryPeek = Query.CreateBuilder();
            queryPeek.Key = StashQueries.PEEK;
            queryPeek.Representations.Add(StashQueries.PEEK);

            var queryPop = Query.CreateBuilder();
            queryPop.Key = StashQueries.POP;
            queryPop.Representations.Add(StashQueries.POP);
            queryPop.Parameters.Add(parName.ToImmutable());

            var queryDrop = Query.CreateBuilder();
            queryDrop.Key = StashQueries.DROP;
            queryDrop.Representations.AddRange(
                new[] { StashQueries.DROP, "remove", "rm" });
            parIdentifier.IsOptional = false;
            queryDrop.Parameters.Add(parIdentifier.ToImmutable());

            var queryClear = Query.CreateBuilder();
            queryClear.Key = StashQueries.CLEAR;
            queryClear.Representations.AddRange(
                new[] { StashQueries.CLEAR, "cls" });

            query.Queries.AddRange(
                new[]
                {
                    queryList.ToImmutable(),
                    queryApply.ToImmutable(),
                    queryPeek.ToImmutable(),
                    queryPop.ToImmutable(),
                    queryDrop.ToImmutable(),
                    queryClear.ToImmutable()
                });

            return query.ToImmutable();
        }

        private static Query CreateApplyQuery()
        {
            var query = Query.CreateBuilder();
            query.Key = ApplyQueries.APPLY;
            query.Representations.AddRange(
                new[] { ApplyQueries.APPLY, "run", "operate", "perform" });

            var parDestinationPath = Parameter.CreateBuilder();
            parDestinationPath.Key = FileManagerParameters.SIMPLE_PATH;
            parDestinationPath.ArgumentTemplate = InputRegex.PATH;
            parDestinationPath.Documentation.Title = "Destination path";
            parDestinationPath.Documentation.Description = "Specify the destination directory.";

            var queryCopy = Query.CreateBuilder();
            queryCopy.Key = ApplyQueries.COPY;
            queryCopy.Representations.AddRange(
                new[] { ApplyQueries.COPY, "cp" });
            queryCopy.Parameters.Add(parDestinationPath.ToImmutable());

            var queryMove = Query.CreateBuilder();
            queryMove.Key = ApplyQueries.MOVE;
            queryMove.Representations.AddRange(
                new[] { ApplyQueries.MOVE, "mv" });
            queryMove.Parameters.Add(parDestinationPath.ToImmutable());

            var queryDelete = Query.CreateBuilder();
            queryDelete.Key = ApplyQueries.DELETE;
            queryDelete.Representations.AddRange(
                new[] { ApplyQueries.DELETE, "remove", "rm", "del", "dl" });

            query.Queries.AddRange(
                new[]
                {
                    queryCopy.ToImmutable(),
                    queryMove.ToImmutable(),
                    queryDelete.ToImmutable(),
                });

            return query.ToImmutable();
        }
    }
}
