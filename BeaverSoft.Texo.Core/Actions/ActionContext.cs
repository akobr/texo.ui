using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Actions
{
    public class ActionContext : IActionContext
    {
        public string Name { get; set; }

        public IDictionary<string, string> Arguments { get; set; }
    }
}