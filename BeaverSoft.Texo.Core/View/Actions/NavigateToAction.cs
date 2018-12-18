using System;
using System.Collections.Generic;
using System.Diagnostics;
using BeaverSoft.Texo.Core.Actions;

namespace BeaverSoft.Texo.Core.View.Actions
{
    public class NavigateToAction : IAction
    {
        private readonly IActionManagementService actionService;

        public NavigateToAction(IActionManagementService actionService)
        {
            this.actionService = actionService;
        }

        public void Execute(IDictionary<string, string> arguments)
        {
            if (!arguments.TryGetValue(ActionParameters.ADDRESS, out string address)
                || string.IsNullOrWhiteSpace(address))
            {
                return;
            }

            Uri uri = TryToBuildUri(address);

            if (uri == null)
            {
                return;
            }

            if (string.Equals(uri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
            {
                actionService.Execute(ActionBuilder.PathUri(uri.LocalPath));
            }
            else if (string.Equals(uri.Scheme, ActionConstants.ACTION_SCHEMA, StringComparison.OrdinalIgnoreCase))
            {
                actionService.Execute(uri.OriginalString);
            }
            else
            {
                Process.Start(uri.OriginalString);
            }
        }

        public Uri TryToBuildUri(string address)
        {
            Uri.TryCreate(address, UriKind.Absolute, out Uri result);
            return result;
        }
    }
}
