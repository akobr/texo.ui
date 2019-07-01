using System.Collections.Generic;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Actions;

namespace BeaverSoft.Texo.Core.View.Actions
{
    public class InputAddAction : IAction
    {
        private readonly IViewService view;

        public InputAddAction(IViewService view)
        {
            this.view = view;
        }

        public Task ExecuteAsync(IDictionary<string, string> arguments)
        {
            if (arguments.TryGetValue(ActionParameters.INPUT, out string input)
                && !string.IsNullOrWhiteSpace(input))
            {
                view.AddInput(input);
            }
            
            return Task.CompletedTask;
        }
    }
}
