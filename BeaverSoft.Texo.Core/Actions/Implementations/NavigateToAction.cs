using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Actions.Implementations
{
    public class NavigateToAction : IAction
    {
        private readonly IActionManagementService actionService;

        public NavigateToAction(IActionManagementService actionService)
        {
            this.actionService = actionService;
        }

        public Task ExecuteAsync(IDictionary<string, string> arguments)
        {
            if (!arguments.TryGetValue(ActionParameters.ADDRESS, out string address)
                || string.IsNullOrWhiteSpace(address))
            {
                return Task.CompletedTask;
            }

            Uri uri = TryToBuildUri(address);

            if (uri == null)
            {
                return Task.CompletedTask;
            }

            if (string.Equals(uri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
            {
                actionService.ExecuteAsync(ActionBuilder.PathUri(uri.LocalPath));
            }
            else if (string.Equals(uri.Scheme, ActionConstants.ACTION_SCHEMA, StringComparison.OrdinalIgnoreCase))
            {
                actionService.ExecuteAsync(uri.OriginalString);
            }
            else
            {
                return UriOpenAction.ExecuteAsync(uri.OriginalString);
            }

            return Task.CompletedTask;
        }

        public Uri TryToBuildUri(string address)
        {
            Uri.TryCreate(address, UriKind.Absolute, out Uri result);
            return result;
        }
    }
}
