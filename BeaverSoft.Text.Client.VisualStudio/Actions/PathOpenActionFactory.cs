using System;
using BeaverSoft.Texo.Core.Actions;

namespace BeaverSoft.Text.Client.VisualStudio.Actions
{
    public class PathOpenActionFactory : IActionFactory
    {
        private readonly ExtensionContext context;

        public PathOpenActionFactory(ExtensionContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IAction Build()
        {
            return new PathOpenAction(context);
        }
    }
}
