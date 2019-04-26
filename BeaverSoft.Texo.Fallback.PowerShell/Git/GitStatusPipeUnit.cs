using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.Transforming;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core.Services;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Fallback.PowerShell.Git
{
    public class GitStatusPipeUnit : ITransformationPipe<FallbackContext>
    {
        private readonly IServiceMessageBus messageBus;

        public GitStatusPipeUnit(IServiceMessageBus messageBus)
        {
            this.messageBus = messageBus;
        }

        public async Task<FallbackContext> ProcessAsync(FallbackContext context)
        {
            if (context.Input.ParsedInput.Tokens.Count < 2
                || !string.Equals(context.Input.ParsedInput.Tokens[0], "git", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(context.Input.ParsedInput.Tokens[1], "status", StringComparison.OrdinalIgnoreCase))
            {
                return context;
            }

            StatusContext status = await Task.Run(() => ProcessGitStatusMessage(context.Result.Text));
            string newPrompt = "tu";

            if (!string.IsNullOrEmpty(status.BranchName))
            {
                newPrompt = $"{status.BranchName} +{status.AddedCount} ~{status.ModifiedCount} -{status.DeletedCount}{GetUntrackedPromptPart(status.ContainsUntracked)}";
            }

            messageBus.Send(new PromptUpdateMessage(newPrompt));
            return context;
        }

        private StatusContext ProcessGitStatusMessage(string message)
        {
            StatusContext context = new StatusContext();

            string[] lines = message.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            Regex branchRegex = new Regex("On\\sbranch\\s(\\S*)");
            Regex fileRegex = new Regex("(new\\sfile|modified|deleted):\\s*([\\S]*)");
            Regex untrackedFileRegex = new Regex("\\s*([\\S]*)");
            
            foreach (string line in lines)
            {
                if (line.StartsWith("\t"))
                {
                    if (line.Contains("new file:"))
                    {
                        context.AddedCount++;
                    }
                    else if (line.Contains("modified:"))
                    {
                        context.ModifiedCount++;
                    }
                    else if (line.Contains("deleted:"))
                    {
                        context.DeletedCount++;
                    }
                }
                else
                {
                    if (line.StartsWith("On branch"))
                    {
                        context.BranchName = branchRegex.Match(line).Groups[1].Value;
                    }
                    else if (line.StartsWith("Untracked files:"))
                    {
                        context.ContainsUntracked = true;
                    }
                }
            }

            return context;
        }

        private string GetUntrackedPromptPart(bool containUntracked)
        {
            return containUntracked ? " !" : string.Empty;
        }

        private class StatusContext
        {
            public string BranchName = string.Empty;
            public MarkdownBuilder Markdown = new MarkdownBuilder();

            public int AddedCount;
            public int ModifiedCount;
            public int DeletedCount;
            public bool ContainsUntracked;
        }
    }
}
