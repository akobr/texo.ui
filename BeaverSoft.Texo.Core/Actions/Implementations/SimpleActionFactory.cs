namespace BeaverSoft.Texo.Core.Actions.Implementations
{
    public class SimpleActionFactory<TAction> : IActionFactory
        where TAction : IAction, new()
    {
        public IAction Build()
        {
            return new TAction();
        }
    }
}
