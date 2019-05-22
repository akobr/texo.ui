using BeaverSoft.Texo.Core.Actions;

namespace BeaverSoft.Texo.Core.View.Actions
{
    public class InputSetActionFactory : IActionFactory
    {
        private IViewService view;

        public InputSetActionFactory(IViewService view)
        {
            this.view = view;
        }

        public IAction Build()
        {
            return new InputSetAction(view);
        }
    }
}
