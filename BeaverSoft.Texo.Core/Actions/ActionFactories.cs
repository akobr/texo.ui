using System.Collections.Generic;
using System.Linq;

namespace BeaverSoft.Texo.Core.Actions
{
    public class ActionFactories : IActionFactoryProvider, IActionFactoryRegister
    {
        private readonly IDictionary<string, IActionFactory> factories;

        public ActionFactories()
        {
            factories = new Dictionary<string, IActionFactory>();
        }

        public void RegisterFactory(IActionFactory factory, params string[] actionNames)
        {
            foreach (string actionName in GetValidActionNames(actionNames))
            {
                factories[actionName] = factory;
            }
        }

        public void UnregisterFactory(params string[] actionNames)
        {
            foreach (var actionName in GetValidActionNames(actionNames))
            {
                factories.Remove(actionName);
            }
        }

        public IActionFactory GetActionFactory(string actionName)
        {
            factories.TryGetValue(actionName, out IActionFactory factory);
            return factory;
        }

        public IEnumerable<string> GetValidActionNames(string[] actionNames)
        {
            if (actionNames == null)
            {
                return Enumerable.Empty<string>();
            }

            return actionNames.Where(name => !string.IsNullOrWhiteSpace(name));
        }
    }
}
