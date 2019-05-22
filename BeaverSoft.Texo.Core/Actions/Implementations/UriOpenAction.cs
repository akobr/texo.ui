using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Actions.Implementations
{
    public class UriOpenAction : IAction
    {
        public Task ExecuteAsync(IDictionary<string, string> arguments)
        {
            if (!arguments.TryGetValue(ActionParameters.URI, out string uri))
            {
                return Task.CompletedTask;
            }

            return ExecuteAsync(uri);
        }

        public static Task ExecuteAsync(string uri)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                Process.Start(uri);
            }

            return Task.CompletedTask;
        }
    }
}
