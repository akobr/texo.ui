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
            command.Queries.Add(BuildCloneQuery());
            command.Queries.Add(BuildBranchQuery());

            command.Options.Add(
                CreateSimpleOption(
                    "version",
                    "Prints the Git suite version that the git program came from."
                ));

            command.Options.Add(
                CreateSimpleOption(
                    "help",
                    "Prints the synopsis and a list of the most commonly used commands. If the option `--all` or `-a` is given then all available commands are printed. If a Git command is named this option will bring up the manual page for that command."
                ));

            return command.ToImmutable();
        }

        private static Query BuildCloneQuery()
        {
            var query = Query.CreateBuilder();
            query.Key = "clone";
            query.Representations.Add("clone");

            query.Documentation.Title = "git-clone";
            query.Documentation.Description = "Clone a repository into a new directory";

            var parRepository = Parameter.CreateBuilder();
            parRepository.Key = "repository";
            parRepository.Documentation.Title = "repository";
            parRepository.Documentation.Description = "URL to repository";

            var parDirectory = Parameter.CreateBuilder();
            parDirectory.Key = "directory";
            parDirectory.IsOptional = true;
            parDirectory.Documentation.Title = "directory";
            parDirectory.Documentation.Description = "Directory to clone to";

            query.Options.Add(
                CreateSimpleOption(
                    "local",
                    "When the repository to clone from is on a local machine, this flag bypasses the normal \"Git aware\" transport mechanism and clones the repository by making a copy of HEAD and everything under objects and refs directories. The files under `.git/objects/` directory are hardlinked to save space when possible.\r\n\r\nIf the repository is specified as a local path(e.g., `/path/to/repo`), this is the default, and `--local` is essentially a no-op. If the repository is specified as a URL, then this flag is ignored (and we never use the local optimizations).Specifying `--no-local` will override the default when `/path/to/repo` is given, using the regular Git transport instead.",
                    "l"
                ));

            query.Options.Add(
                CreateSimpleOption(
                    "no-hardlinks",
                    "Force the cloning process from a repository on a local filesystem to copy the files under the `.git/objects` directory instead of using hardlinks. This may be desirable if you are trying to make a back-up of your repository."
                ));

            return query.ToImmutable();
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
