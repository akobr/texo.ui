using System;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Runtime;

namespace BeaverSoft.Texo.Test.Client.WPF.Actions
{
    public class PathOpenActionFactory : IActionFactory
    {
        private readonly IExecutor executor;

        public PathOpenActionFactory(IExecutor executor)
        {
            this.executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public IAction Build()
        {
            return new PathOpenAction(executor);
        }
    }
}
