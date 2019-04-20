using BeaverSoft.Texo.Core.Actions;

namespace BeaverSoft.Texo.Test.Client.WPF.Actions
{
    public class PathOpenActionFactory : IActionFactory
    {
        public IAction Build()
        {
            return new PathOpenAction();
        }
    }
}
