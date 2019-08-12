using System;
using System.Collections.Generic;
using System.Text;
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

        public Task<IEnumerable<IItem>> GetHelpAsync(Input input)
        {
            throw new NotImplementedException();
        }
    }
}
