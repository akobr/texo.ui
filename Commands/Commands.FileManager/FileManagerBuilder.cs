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
            command.Documentation.Title = "File manager";
            command.Documentation.Description = "Command line file manager with stash(es).";

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
            parDestinationPath.Documentation.Description = "Specify the destination directory/file.";

            var parSearchTerm = Parameter.CreateBuilder();
            parSearchTerm.Key = ApplyParameters.SEARCH_TERM;
            parSearchTerm.Documentation.Title = "Search term";
            parSearchTerm.Documentation.Description = "Search term or regular expression.";

            var optionPreview = Option.CreateBuilder();
            optionPreview.Key = ApplyOptions.PREVIEW;
            optionPreview.Representations.AddRange(
                new[] { ApplyOptions.PREVIEW, "p" });
            optionPreview.Documentation.Title = ApplyOptions.PREVIEW;
            optionPreview.Documentation.Description = "Do only preview of the operation.";

            var optionOverwrite = Option.CreateBuilder();
            optionOverwrite.Key = ApplyOptions.OVERWRITE;
            optionOverwrite.Representations.AddRange(
                new[] { ApplyOptions.OVERWRITE, "o" });
            optionOverwrite.Documentation.Title = ApplyOptions.OVERWRITE;
            optionOverwrite.Documentation.Description = "Overwrite target files.";

            var optionAdd = Option.CreateBuilder();
            optionAdd.Key = ApplyOptions.ADD;
            optionAdd.Representations.AddRange(
                new[] { ApplyOptions.ADD, "a" });
            optionAdd.Documentation.Title = ApplyOptions.ADD;
            optionAdd.Documentation.Description = "Add files to an existing zip archive.";

            var optionFlatten = Option.CreateBuilder();
            optionFlatten.Key = ApplyOptions.FLATTEN;
            optionFlatten.Representations.AddRange(
                new[] { ApplyOptions.FLATTEN, "f" });
            optionFlatten.Documentation.Title = ApplyOptions.FLATTEN;
            optionFlatten.Documentation.Description = "Flat all directories and files.";

            var optionRegex = Option.CreateBuilder();
            optionRegex.Key = ApplyOptions.REGEX;
            optionRegex.Representations.AddRange(
                new[] { ApplyOptions.REGEX, "r" });
            optionRegex.Documentation.Title = ApplyOptions.REGEX;
            optionRegex.Documentation.Description = "Search as a regular expression.";

            var optionCaseSensitive = Option.CreateBuilder();
            optionCaseSensitive.Key = ApplyOptions.CASE_SENSITIVE;
            optionCaseSensitive.Representations.AddRange(
                new[] { ApplyOptions.CASE_SENSITIVE, "cs" });
            optionCaseSensitive.Documentation.Title = ApplyOptions.CASE_SENSITIVE;
            optionCaseSensitive.Documentation.Description = "Make search case sensitive.";

            var optionFileFilter = Option.CreateBuilder();
            optionFileFilter.Key = ApplyOptions.FILE_FILTER;
            optionFileFilter.Representations.AddRange(
                new[] { ApplyOptions.FILE_FILTER, "ff" });
            optionFileFilter.Documentation.Title = ApplyOptions.FILE_FILTER;
            optionFileFilter.Documentation.Description = "Specify a file filter.";

            var queryCopy = Query.CreateBuilder();
            queryCopy.Key = ApplyQueries.COPY;
            queryCopy.Representations.AddRange(
                new[] { ApplyQueries.COPY, "cp" });
            queryCopy.Parameters.Add(parDestinationPath.ToImmutable());
            queryCopy.Options.Add(optionPreview.ToImmutable());
            queryCopy.Options.Add(optionOverwrite.ToImmutable());
            queryCopy.Options.Add(optionFlatten.ToImmutable());

            var queryMove = Query.CreateBuilder();
            queryMove.Key = ApplyQueries.MOVE;
            queryMove.Representations.AddRange(
                new[] { ApplyQueries.MOVE, "mv" });
            queryMove.Parameters.Add(parDestinationPath.ToImmutable());
            queryMove.Options.Add(optionPreview.ToImmutable());
            queryMove.Options.Add(optionOverwrite.ToImmutable());
            queryMove.Options.Add(optionFlatten.ToImmutable());

            var queryDelete = Query.CreateBuilder();
            queryDelete.Key = ApplyQueries.DELETE;
            queryDelete.Representations.AddRange(
                new[] { ApplyQueries.DELETE, "remove", "rm", "del", "dl" });
            queryDelete.Options.Add(optionPreview.ToImmutable());

            var querySearch = Query.CreateBuilder();
            querySearch.Key = ApplyQueries.SEARCH;
            querySearch.Representations.AddRange(
                new[] { ApplyQueries.SEARCH, "s", "find" });
            querySearch.Parameters.Add(parSearchTerm.ToImmutable());
            querySearch.Options.Add(optionRegex.ToImmutable());
            querySearch.Options.Add(optionCaseSensitive.ToImmutable());
            querySearch.Options.Add(optionFileFilter.ToImmutable());

            var queryArchive = Query.CreateBuilder();
            queryArchive.Key = ApplyQueries.ARCHIVE;
            queryArchive.Representations.AddRange(
                new[] { ApplyQueries.ARCHIVE, "zip" });
            queryArchive.Parameters.Add(parDestinationPath.ToImmutable());
            queryArchive.Options.Add(optionFlatten.ToImmutable());
            queryArchive.Options.Add(optionAdd.ToImmutable());
            queryArchive.Options.Add(optionOverwrite.ToImmutable());

            query.Queries.AddRange(
                new[]
                {
                    queryCopy.ToImmutable(),
                    queryMove.ToImmutable(),
                    queryDelete.ToImmutable(),
                    querySearch.ToImmutable(),
                    queryArchive.ToImmutable(),
                });

            return query.ToImmutable();
        }
    }
}
