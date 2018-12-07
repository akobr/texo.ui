using System;
using BeaverSoft.Texo.Core.Configuration;

namespace Commands.Git
{
    public static class GitBuilder
    {
        public static Query BuildCommand()
        {
            var command = Query.CreateBuilder();
            command.Key = "git";
            command.Representations.Add("git");
            command.Documentation.Description = "Delete a branch. The branch must be fully merged in its upstream branch, or in `HEAD` if no upstream was set with `--track` or `--set-upstream-to`.";

            command.Queries.Add(BuildBranchQuery());
        }

        private static Query BuildBranchQuery()
        {
            var query = Query.CreateBuilder();
            query.Key = "branch";
            query.Representations.Add("branch");

            query.Documentation.Title = "git-branch";
            query.Documentation.Description = "List, create, or delete branches";

            var delete = Option.CreateBuilder();
            delete.Key = "delete";
            delete.Representations.Add("delete");
            delete.Documentation.Title = "delete";
            delete.Documentation.Description = "Delete a branch. The branch must be fully merged in its upstream branch, or in `HEAD` if no upstream was set with `--track` or `--set-upstream-to`.";

            var force = Option.CreateBuilder();
            force.Key = "force";
            force.Representations.Add("force");
            force.Documentation.Title = "force";
            force.Documentation.Description = "Reset <branchname> to <startpoint>, even if <branchname> exists already. Without `-f`, git branch refuses to change an existing branch. In combination with `-d` (or `--delete`), allow deleting the branch irrespective of its merged status. In combination with `-m` (or `--move`), allow renaming the branch even if the new branch name already exists, the same applies for `-c` (or `--copy`).";

            var move = Option.CreateBuilder();
            move.Key = "move";
            move.Representations.Add("move");
            move.Documentation.Title = "move";
            move.Documentation.Description = "Move/rename a branch and the corresponding reflog.";

            var copy = Option.CreateBuilder();
            copy.Key = "copy";
            copy.Representations.Add("copy");
            copy.Documentation.Title = "copy";
            copy.Documentation.Description = "Copy a branch and the corresponding reflog.";
        }
    }
}
