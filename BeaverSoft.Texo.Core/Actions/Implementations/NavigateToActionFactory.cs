namespace BeaverSoft.Texo.Core.Actions.Implementations
{
    public class NavigateToActionFactory : IActionFactory
    {
        private readonly IActionManagementService actionService;

        public NavigateToActionFactory(IActionManagementService actionService)
        {
            this.actionService = actionService;
        }

        public IAction Build()
        {
            return new NavigateToAction(actionService);
        }
    }
}
