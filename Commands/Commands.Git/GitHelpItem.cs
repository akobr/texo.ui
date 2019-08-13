using System.Collections.Generic;

namespace Commands.Git
{
    public class GitHelpItem
    {
        public GitHelpItem()
        {
            Children = new SortedDictionary<string, GitHelpItem>();   
        }

        public string Name { get; set; }

        public string Input { get; set; }

        public string Description { get; set; }

        public IDictionary<string, GitHelpItem> Children { get; }
    }
}
