using BeaverSoft.Texo.Core.Actions;

namespace BeaverSoft.Texo.Core.View.Actions
{
    public class InputAddActionFactory
    {
        private IViewService view;

        public InputAddActionFactory(IViewService view)
        {
            this.view = view;
        }

        public IAction Build()
        {
            return new InputAddAction(view);
        }
    }
}
