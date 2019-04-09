using System;
using System.Collections.Generic;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Core.Actions
{
    public class ActionManagementService : IActionManagementService
    {
        private readonly IActionFactoryProvider provider;
        private readonly IActionUrlParser parser;
        private readonly ILogService logger;

        public ActionManagementService(
            IActionFactoryProvider provider,
            ILogService logger)
        {
            parser = new ActionUrlParser();

            this.provider = provider;
            this.logger = logger;
        }

        public void Execute(string actionUrl)
        {
            IActionContext context = parser.Parse(actionUrl);

            if (context == null)
            {
                return;
            }

            Execute(context.Name, context.Arguments);
        }

        public void Execute(string actionName, IDictionary<string, string> arguments)
        {
            IActionFactory factory = provider?.GetActionFactory(actionName);

            if (factory == null)
            {
                logger.Warn($"No action factory for action: {actionName}.", arguments);
                return;
            }

            IAction action = CreateActionInstance(factory);

            if (action == null)
            {
                logger.Warn($"No action created for action: {actionName}.", factory, arguments);
                return;
            }

            ExecuteAction(action, arguments);
        }

        private void ExecuteAction(IAction action, IDictionary<string, string> arguments)
        {
            try
            {
                action.Execute(arguments);
            }
            catch (Exception)
            {
                logger.Error("Error during excution of action.", action, arguments);
            }
        }

        private IAction CreateActionInstance(IActionFactory factory)
        {
            try
            {
                return factory.Build();
            }
            catch (Exception exception)
            {
                logger.Warn($"Error on action instanciation.", factory, exception);
                return null;
            }
        }
    }
}
