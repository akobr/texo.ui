using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Runtime;

namespace BeaverSoft.Text.Client.VisualStudio.Actions
{
    public class PathOpenActionFactory : IActionFactory
    {
        private readonly EnvDTE80.DTE2 dte;
        private readonly IExecutor executor;

        public PathOpenActionFactory(EnvDTE80.DTE2 dte, IExecutor executor)
        {
            this.dte = dte;
            this.executor = executor;
        }

        public IAction Build()
        {
            return new PathOpenAction(dte, executor);
        }
    }
}
