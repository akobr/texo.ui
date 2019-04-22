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
            command.Documentation.Description = "Command line file manager with stage and stash(es).";

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
            query.Documentation.Title = "file-manager-stage";
            query.Documentation.Description = "Manage the stage of file manager.";

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
            queryStatus.Documentation.Title = "file-manager-stage-status";
            queryStatus.Documentation.Description = "Status of the stage in file manager.";

            var queryList = Query.CreateBuilder();
            queryList.Key = StageQueries.LIST;
            queryList.Representations.AddRange(
                new[] { StageQueries.LIST, "dir", "ls" });
            queryList.Documentation.Title = "file-manager-stage-list";
            queryList.Documentation.Description = "List of current working directory.";
            parPath.IsOptional = true;
            queryList.Parameters.Add(parPath.ToImmutable());

            var queryAdd = Query.CreateBuilder();
            queryAdd.Key = StageQueries.ADD;
            queryAdd.Representations.Add(StageQueries.ADD);
            queryAdd.Documentation.Title = "file-manager-stage-add";
            queryAdd.Documentation.Description = "Adds new files/directories to the stage of file manager.";
            parPath.IsOptional = false;
            queryAdd.Parameters.Add(parPath.ToImmutable());

            var queryRemove = Query.CreateBuilder();
            queryRemove.Key = StageQueries.REMOVE;
            queryRemove.Representations.AddRange(
                new[] { StageQueries.REMOVE, "rm" });
            queryRemove.Documentation.Title = "file-manager-stage-remove";
            queryRemove.Documentation.Description = "Removes files/directories from the stage of file manager.";
            queryRemove.Parameters.Add(parPath.ToImmutable());

            var queryLobby = Query.CreateBuilder();
            queryLobby.Key = StageQueries.LOBBY;
            queryLobby.Representations.Add(StageQueries.LOBBY);
            queryLobby.Documentation.Title = "file-manager-stage-lobby";
            queryLobby.Documentation.Description = "Sets a lobby to the stage of file manager.";
            queryLobby.Parameters.Add(parLobbyPath.ToImmutable());

            var queryRemoveLobby = Query.CreateBuilder();
            queryRemoveLobby.Key = StageQueries.REMOVE_LOBBY;
            queryRemoveLobby.Representations.AddRange(
                new[] { StageQueries.REMOVE_LOBBY, "rm-lobby", "rml" });
            queryRemoveLobby.Documentation.Title = "file-manager-stage-remove-lobby";
            queryRemoveLobby.Documentation.Description = "Removes a lobby from the stage of file manager.";
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
            query.DefaultQueryKey = StashQueries.PUSH;
            query.Documentation.Title = "file-manager-stash";
            query.Documentation.Description = "Manage the stack fo stashes in file manager.";

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
            queryList.Documentation.Title = "file-manager-stash-list";
            queryList.Documentation.Description = "List of available stashes in the stack.";

            var queryPush = Query.CreateBuilder();
            queryPush.Key = StashQueries.PUSH;
            queryPush.Representations.Add(StashQueries.PUSH);
            queryPush.Documentation.Title = "file-manager-stash-push";
            queryPush.Documentation.Description = "Adds a stash to the stack.";
            queryPush.Parameters.Add(parName.ToImmutable());

            var queryApply = Query.CreateBuilder();
            queryApply.Key = StashQueries.APPLY;
            queryApply.Representations.Add(StashQueries.APPLY);
            queryApply.Documentation.Title = "file-manager-stash-apply";
            queryApply.Documentation.Description = "Applies specific stash to the stage of file manager.";
            queryApply.Parameters.Add(parIdentifier.ToImmutable());

            var queryPeek = Query.CreateBuilder();
            queryPeek.Key = StashQueries.PEEK;
            queryPeek.Representations.Add(StashQueries.PEEK);
            queryPeek.Documentation.Title = "file-manager-stash-peek";
            queryPeek.Documentation.Description = "Applies top stash to the stage of file manager. A stash won't be removed.";

            var queryPop = Query.CreateBuilder();
            queryPop.Key = StashQueries.POP;
            queryPop.Representations.Add(StashQueries.POP);
            queryPop.Documentation.Title = "file-manager-stash-pop";
            queryPop.Documentation.Description = "Applies and remove top stash to the stage of file manager.";

            var queryDrop = Query.CreateBuilder();
            queryDrop.Key = StashQueries.DROP;
            queryDrop.Representations.AddRange(
                new[] { StashQueries.DROP, "remove", "rm" });
            queryDrop.Documentation.Title = "file-manager-stash-drop";
            queryDrop.Documentation.Description = "Removes specific stash from the stage of file manager.";
            parIdentifier.IsOptional = false;
            queryDrop.Parameters.Add(parIdentifier.ToImmutable());

            var queryClear = Query.CreateBuilder();
            queryClear.Key = StashQueries.CLEAR;
            queryClear.Representations.AddRange(
                new[] { StashQueries.CLEAR, "cls" });
            queryClear.Documentation.Title = "file-manager-stash-clear";
            queryClear.Documentation.Description = "Removes all stashes from the stack of file manager.";

            query.Queries.AddRange(
                new[]
                {
                    queryList.ToImmutable(),
                    queryPush.ToImmutable(),
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
            query.Documentation.Title = "file-manager-apply";
            query.Documentation.Description = "Perform an operation in file manager.";

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
            optionAdd.Documentation.Description = "Adds files to an existing zip archive.";

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

            var queryCopy = Query.CreateBuilder();
            queryCopy.Key = ApplyQueries.COPY;
            queryCopy.Representations.AddRange(
                new[] { ApplyQueries.COPY, "cp" });
            queryCopy.Documentation.Title = "file-manager-copy";
            queryCopy.Documentation.Description = "A copy operation.";
            queryCopy.Parameters.Add(parDestinationPath.ToImmutable());
            queryCopy.Options.Add(optionPreview.ToImmutable());
            queryCopy.Options.Add(optionOverwrite.ToImmutable());
            queryCopy.Options.Add(optionFlatten.ToImmutable());

            var queryMove = Query.CreateBuilder();
            queryMove.Key = ApplyQueries.MOVE;
            queryMove.Representations.AddRange(
                new[] { ApplyQueries.MOVE, "mv" });
            queryMove.Documentation.Title = "file-manager-move";
            queryMove.Documentation.Description = "A move operation.";
            queryMove.Parameters.Add(parDestinationPath.ToImmutable());
            queryMove.Options.Add(optionPreview.ToImmutable());
            queryMove.Options.Add(optionOverwrite.ToImmutable());
            queryMove.Options.Add(optionFlatten.ToImmutable());

            var queryDelete = Query.CreateBuilder();
            queryDelete.Key = ApplyQueries.DELETE;
            queryDelete.Representations.AddRange(
                new[] { ApplyQueries.DELETE, "remove", "rm", "del", "dl" });
            queryDelete.Documentation.Title = "file-manager-delete";
            queryDelete.Documentation.Description = "A delete operation.";
            queryDelete.Options.Add(optionPreview.ToImmutable());

            var querySearch = Query.CreateBuilder();
            querySearch.Key = ApplyQueries.SEARCH;
            querySearch.Representations.AddRange(
                new[] { ApplyQueries.SEARCH, "s", "find" });
            querySearch.Documentation.Title = "file-manager-search";
            querySearch.Documentation.Description = "A content search operation.";
            querySearch.Parameters.Add(parSearchTerm.ToImmutable());
            querySearch.Options.Add(optionRegex.ToImmutable());
            querySearch.Options.Add(optionCaseSensitive.ToImmutable());

            var queryArchive = Query.CreateBuilder();
            queryArchive.Key = ApplyQueries.ARCHIVE;
            queryArchive.Representations.AddRange(
                new[] { ApplyQueries.ARCHIVE, "zip" });
            queryArchive.Documentation.Title = "file-manager-archive";
            queryArchive.Documentation.Description = "An archive (zip) operation.";
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
