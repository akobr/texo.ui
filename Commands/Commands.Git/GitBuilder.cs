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
            command.Documentation.Title = "Git CLI";
            command.Documentation.Description = "the stupid content tracker";

            command.Queries.Add(BuildHelpQuery());
            command.Queries.Add(BuildBranchQuery());

            return command.ToImmutable();
        }

        private static Query BuildBranchQuery()
        {
            var query = Query.CreateBuilder();
            query.Key = "branch";
            query.Representations.Add("branch");

            query.Documentation.Title = "git-branch";
            query.Documentation.Description = "List, create, or delete branches";

            query.Options.Add(
                CreateSimpleOption(
                    "delete",
                    "Delete a branch. The branch must be fully merged in its upstream branch, or in `HEAD` if no upstream was set with `--track` or `--set-upstream-to`.",
                    "d"
                ));

            query.Options.Add(
                CreateSimpleOption(
                    "force",
                    "Reset <branchname> to <startpoint>, even if <branchname> exists already. Without `-f`, git branch refuses to change an existing branch. In combination with `-d` (or `--delete`), allow deleting the branch irrespective of its merged status. In combination with `-m` (or `--move`), allow renaming the branch even if the new branch name already exists, the same applies for `-c` (or `--copy`).",
                    "f"
                ));

            query.Options.Add(
                CreateSimpleOption(
                    "move",
                    "Move/rename a branch and the corresponding reflog.",
                    "m"
                ));

            query.Options.Add(
                CreateSimpleOption(
                    "copy",
                    "Copy a branch and the corresponding reflog.",
                    "c"
                ));

            return query.ToImmutable();
        }

        private static Query BuildHelpQuery()
        {
            var query = Query.CreateBuilder();
            query.Key = "help";
            query.Representations.Add("help");

            query.Documentation.Title = "git-help";
            query.Documentation.Description = "Display help information about Git";

            var par = Parameter.CreateBuilder();
            par.Key = "command";
            par.IsOptional = true;
            par.IsRepeatable = true;
            query.Parameters.Add(par.ToImmutable());

            query.Options.Add(
                CreateSimpleOption(
                    "all",
                    "Prints all the available commands on the standard output. This option overrides any given command or guide name. When used with `--verbose` print description for all recognized commands.",
                    "a"
                ));

            query.Options.Add(
                CreateSimpleOption(
                    "config",
                    "List all available configuration variables. This is a short summary of the list in [git-config[1]](https://git-scm.com/docs/git-config).",
                    "c"
                ));

            query.Options.Add(
                CreateSimpleOption(
                    "guides",
                    "Prints a list of useful guides on the standard output. This option overrides any given command or guide name.",
                    "g"
                ));

            query.Options.Add(
                CreateSimpleOption(
                    "info",
                    "Display manual page for the command in the *info* format. The *info* program will be used for that purpose.",
                    "i"
                ));

            query.Options.Add(
                CreateSimpleOption(
                    "man",
                    "Display manual page for the command in the man format. This option may be used to override a value set in the `help.format` configuration variable. By default the *man* program will be used to display the manual page, but the `man.viewer` configuration variable may be used to choose other display programs(see below).",
                    "m"
                ));

            query.Options.Add(
                CreateSimpleOption(
                    "web",
                    "Display manual page for the command in the web (HTML) format. A web browser will be used for that purpose. The web browser can be specified using the configuration variable `help.browser`, or `web.browser` if the former is not set.",
                    "w"
                ));

            return query.ToImmutable();
        }

        private static Option CreateSimpleOption(string name, string description, params string[] representations)
        {
            var option = Option.CreateBuilder();
            option.Key = name;
            option.Representations.Add(name);
            option.Representations.AddRange(representations);
            option.Documentation.Title = name;
            option.Documentation.Description = description;

            return option.ToImmutable();
        }
    }
}
