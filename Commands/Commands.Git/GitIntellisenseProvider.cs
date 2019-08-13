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
            items.Add("log", BuildLogHelp());

            // TODO: push, pull, remote, stash, merge, checkout, reset, rm, mv, diff, add

            items.Add("init", new GitHelpItem() { Name = "init [project-name]", Input = "init", Description = "initialize a local Git repository" });
            items.Add("clone", new GitHelpItem() { Name = "clone [repository-url]", Input = "clone", Description = "create a local copy of a remote repository" });
            items.Add("status", new GitHelpItem() { Name = "status", Input = "status", Description = "check status" });
            items.Add("fetch", new GitHelpItem() { Name = "fetch [bookmark]", Input = "fetch", Description = "downloads all history from the repository bookmark" });
            items.Add("show", new GitHelpItem() { Name = "show [commit]", Input = "show", Description = "outputs metadata and content changes of the specified commit" });
            items.Add("ls-files", new GitHelpItem() { Name = "ls-files --other --ignored --exclude-standard", Input = "ls-files --other --ignored --exclude-standard", Description = "lists all ignored files in this project" });
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
            branch.Children.Add("", new GitHelpItem() { Name = "[branch-name]", Input = "", Description = "create a new branch" });
            branch.Children.Add("-a", new GitHelpItem() { Name = "-a", Input = "-a", Description = "list of all branches (local and remote)" });
            branch.Children.Add("-d", new GitHelpItem() { Name = "-d [branch-name]", Input = "-d", Description = "delete a branch" });
            branch.Children.Add("-m", new GitHelpItem() { Name = "-m [old-branch-name] [new-branch-name]", Input = "-m", Description = "rename a local branch" });
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
