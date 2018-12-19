using StrongBeaver.Core.Container;

namespace BeaverSoft.Texo.Core.Actions
{
    public class SimpleIocActionFactoryProvider : IActionFactoryProvider
    {
        private readonly SimpleIoc container;

        public SimpleIocActionFactoryProvider(SimpleIoc container)
        {
            this.container = container;
        }

        public IActionFactory GetActionFactory(string actionName)
        {
            if (!container.IsRegistered<IActionFactory>(actionName))
            {
                return null;
            }

            return container.GetInstance<IActionFactory>(actionName);
        }
    }
}
