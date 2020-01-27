using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.Intellisense;
using BeaverSoft.Texo.Core.View;

namespace Commands.Git
{
    public class GitIntellisenseProvider : IIntellisenseProvider
    {
        // https://github.com/joshnh/Git-Commands
        // https://github.github.com/training-kit/downloads/github-git-cheat-sheet.pdf

        private readonly IDictionary<string, GitHelpItem> items;

        public GitIntellisenseProvider()
        {
            items = new SortedDictionary<string, GitHelpItem>();
            CreateHelpModel();
        }

        public Task<IEnumerable<IItem>> GetHelpAsync(Input input)
        {
            if (input.ParsedInput.RawInput.Length > 50
                || input.ParsedInput.Tokens.Count > 3)
            {
                return Task.FromResult(Enumerable.Empty<IItem>());
            }

            if (input.ParsedInput.Tokens.Count == 1)
            {
                return Task.FromResult(GetHelp(items.Values));
            }
            else if (input.ParsedInput.Tokens.Count == 2
                && items.ContainsKey(input.ParsedInput.Tokens[1]))
            {
                return Task.FromResult(GetHelp(items[input.ParsedInput.Tokens[1]].Children.Values));
            }
            else
            {
                return Task.FromResult(Enumerable.Empty<IItem>());
            }
        }

        private IEnumerable<IItem> GetHelp(IEnumerable<GitHelpItem> items)
        {
            foreach (GitHelpItem item in items)
            {
                yield return Item.AsIntellisense(item.Name, item.Input, null, item.Description);
            }
        }

        private void CreateHelpModel()
        {
            items.Add("config", BuildConfigHelp());
            items.Add("branch", BuildBranchHelp());
            items.Add("add", BuildAddHelp());
            items.Add("push", BuildPushHelp());
            items.Add("pull", BuildPullHelp());
            items.Add("checkout", BuildCheckoutHelp());
            items.Add("merge", BuildMergeHelp());
            items.Add("stash", BuildStashHelp());
            items.Add("diff", BuildDiffHelp());
            items.Add("log", BuildLogHelp());
            items.Add("rm", BuildRmHelp());

            items.Add("commit", new GitHelpItem() { Name = "commit -m \"[descriptive message]\"", Input = "commit -m", Description = "records file snapshots permanently in version history" });
            items.Add("init", new GitHelpItem() { Name = "init [project-name]", Input = "init", Description = "initialise a local Git repository" });
            items.Add("clone", new GitHelpItem() { Name = "clone [repository-url] [folder]", Input = "clone", Description = "creates a local copy of a remote repository" });
            items.Add("status", new GitHelpItem() { Name = "status", Input = "status", Description = "check status" });
            items.Add("fetch", new GitHelpItem() { Name = "fetch [bookmark]", Input = "fetch", Description = "downloads all history from the repository bookmark" });
            items.Add("show", new GitHelpItem() { Name = "show [commit]", Input = "show", Description = "outputs metadata and content changes of the specified commit" });
            items.Add("reset", new GitHelpItem() { Name = "reset [file]", Input = "reset", Description = "unstages the file, but preserve its contents" });
            items.Add("mv", new GitHelpItem() { Name = "mv [file-original] [file-renamed]", Input = "mv", Description = "changes the file name and prepares it for commit" });
            items.Add("ls-files", new GitHelpItem() { Name = "ls-files --other --ignored --exclude-standard", Input = "ls-files --other --ignored --exclude-standard", Description = "lists all ignored files in this project" });
            items.Add("remote", new GitHelpItem() { Name = "remote add [remote-name] [repository-url]", Input = "remote add", Description = "adds a remote repository" });
            items.Add("show-branch", new GitHelpItem() { Name = "show-branch [branch1] [branch2]", Input = "show-branch", Description = "shows branches and their commits (ancestry graph)" });
            items.Add("cherry-pick", new GitHelpItem() { Name = "cherry-pick [commit]", Input = "cherry-pick", Description = "applies the changes introduced by some existing commit(s)" });
        }

        private GitHelpItem BuildAddHelp()
        {
            GitHelpItem add = new GitHelpItem() { Name = "add [file]", Input = "add", Description = "adds a file to the staging area" };
            add.Children.Add("-A", new GitHelpItem() { Name = "-A", Input = "-A", Description = "adds all new and changed files to the staging area" });
            add.Children.Add("--patch", new GitHelpItem() { Name = "--patch", Input = "--patch", Description = "interactively choose hunks of patch between the stagging and the work tree" });
            return add;
        }

        private GitHelpItem BuildDiffHelp()
        {
            GitHelpItem diff = new GitHelpItem() { Name = "diff", Input = "diff", Description = "shows file differences not yet staged" };
            diff.Children.Add("", new GitHelpItem() { Name = "[source-branch] [target-branch]", Input = "", Description = "shows preview changes before merging" });
            diff.Children.Add("--staged", new GitHelpItem() { Name = "--staged", Input = "--staged", Description = "shows file differences between staging and the last file version" });
            return diff;
        }

        private GitHelpItem BuildPullHelp()
        {
            GitHelpItem pull = new GitHelpItem() { Name = "pull", Input = "pull", Description = "downloads bookmark history and incorporates changes" };
            pull.Children.Add("origin", new GitHelpItem() { Name = "origin [branch]", Input = "origin", Description = "pulls changes from remote repository" });
            return pull;
        }

        private GitHelpItem BuildPushHelp()
        {
            GitHelpItem push = new GitHelpItem() { Name = "push", Input = "push", Description = "pushes changes to remote repository (remembered branch)" };
            push.Children.Add("", new GitHelpItem() { Name = "[alias] [branch]", Input = "", Description = "uploads all local branch commits to remote Git" });
            push.Children.Add("origin --delete", new GitHelpItem() { Name = "origin --delete [branch]", Input = "origin --delete", Description = "deletes a remote branch" });
            return push;
        }

        private GitHelpItem BuildCheckoutHelp()
        {
            GitHelpItem checkout = new GitHelpItem() { Name = "checkout [branch]", Input = "checkout", Description = "switches to the specified branch and updates the working directory" };
            checkout.Children.Add("-", new GitHelpItem() { Name = "-", Input = "-", Description = "switches to the branch last checked out" });
            checkout.Children.Add("--", new GitHelpItem() { Name = "-- [file]", Input = "--", Description = "discards changes to a file/files" });
            checkout.Children.Add("-b$1", new GitHelpItem() { Name = "-b [branch]", Input = "-b", Description = "creates a new branch and switches to it" });
            checkout.Children.Add("-b$2", new GitHelpItem() { Name = "-b [branch] origin/[branch]", Input = "-b", Description = "clones a remote branch and switches to it" });
            return checkout;
        }

        private GitHelpItem BuildMergeHelp()
        {
            GitHelpItem merge = new GitHelpItem() { Name = "merge [branch]", Input = "merge", Description = "merges a branch into the active branch" };
            merge.Children.Add("$a", new GitHelpItem() { Name = "[source-branch] [target-branch]", Input = "", Description = "merges a source branch into a target branch" });
            merge.Children.Add("$b", new GitHelpItem() { Name = "[bookmark]/[branch]", Input = "", Description = "combines bookmark’s branch into current local branch" });
            return merge;
        }

        private GitHelpItem BuildStashHelp()
        {
            GitHelpItem stash = new GitHelpItem() { Name = "stash", Input = "stash", Description = "temporarily stores all modified tracked files" };
            stash.Children.Add("pop", new GitHelpItem() { Name = "pop", Input = "pop", Description = "restores the most recently stashed files" });
            stash.Children.Add("list", new GitHelpItem() { Name = "list", Input = "list", Description = "lists all stashed changesets" });
            stash.Children.Add("drop", new GitHelpItem() { Name = "drop", Input = "drop", Description = "discards the most recently stashed changeset" });
            stash.Children.Add("clear", new GitHelpItem() { Name = "clear", Input = "clear", Description = "removes all stashed entries" });
            return stash;
        }

        private GitHelpItem BuildRmHelp()
        {
            GitHelpItem rm = new GitHelpItem() { Name = "rm", Input = "rm", Description = "removes a file (or folder)" };
            rm.Children.Add("", new GitHelpItem() { Name = "[file]", Input = "", Description = "deletes the file from the working directory and stages the deletion" });
            rm.Children.Add("-r", new GitHelpItem() { Name = "-r [folder]", Input = "-r", Description = "deletes the folder (recursively)" });
            rm.Children.Add("--cached", new GitHelpItem() { Name = "--cached [file]", Input = "--cached", Description = "removes the file from version control but preserves the file locally" });
            return rm;
        }

        private GitHelpItem BuildConfigHelp()
        {
            GitHelpItem config = new GitHelpItem() { Name = "config", Input = "config", Description = "configure user information for all local repositories" };
            config.Children.Add("--global user.name \"[name]\"", new GitHelpItem() { Name = "--global user.name \"[name]\"", Input = "--global user.name", Description = "sets the name you want atached to your commit transactions" });
            config.Children.Add("--global user.email  \"[email]\"", new GitHelpItem() { Name = "--global user.email \"[email]\"", Input = "--global user.email", Description = "sets the email you want atached to your commit transactions" });
            config.Children.Add("--global color.ui auto", new GitHelpItem() { Name = "--global color.ui auto", Input = "--global color.ui auto", Description = "enables helpful colorization of command line output" });
            return config;
        }

        private GitHelpItem BuildBranchHelp()
        {
            GitHelpItem branch = new GitHelpItem() { Name = "branch", Input = "branch", Description = "list of branches" };
            branch.Children.Add("", new GitHelpItem() { Name = "[branch]", Input = "", Description = "creates a new branch" });
            branch.Children.Add("-a", new GitHelpItem() { Name = "-a", Input = "-a", Description = "list of all branches (local and remote)" });
            branch.Children.Add("-d", new GitHelpItem() { Name = "-d [branch]", Input = "-d", Description = "deletes a branch" });
            branch.Children.Add("-m", new GitHelpItem() { Name = "-m [old-branch] [new-branch]", Input = "-m", Description = "renames a local branch" });
            return branch;
        }

        private GitHelpItem BuildLogHelp()
        {
            GitHelpItem branch = new GitHelpItem() { Name = "log", Input = "log", Description = "lists version history for the current branch" };
            branch.Children.Add("--follow", new GitHelpItem() { Name = "--follow [file]", Input = "--follow", Description = "lists version history for a file, including renames" });
            branch.Children.Add("--summary", new GitHelpItem() { Name = "--summary", Input = "--summary", Description = "lists version history for the current branch (detailed)" });
            return branch;
        }

    }
}
