using System;
using System.IO;
using BeaverSoft.Texo.Core.Commands;

namespace Commands.SpinSport
{
    public abstract class BaseSolutionCommand : InlineIntersectionCommand
    {
        private readonly ISolutionDirectoryProvider solutionProvider;

        protected BaseSolutionCommand(ISolutionDirectoryProvider solutionProvider)
        {
            this.solutionProvider = solutionProvider ?? throw new ArgumentNullException(nameof(solutionProvider));
        }

        protected string GetSolutionPath()
        {
            return solutionProvider.Get();
        }

        protected string CombineWithSolutionPath(string relativePath)
        {
            return Path.Combine(solutionProvider.Get(), relativePath);
        }
    }
}
