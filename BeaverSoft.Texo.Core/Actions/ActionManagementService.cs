using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public Task ExecuteAsync(string actionUrl)
        {
            IActionContext context = parser.Parse(actionUrl);

            if (context == null)
            {
                return Task.CompletedTask;
            }

            return ExecuteAsync(context.Name, context.Arguments);
        }

        public Task ExecuteAsync(string actionName, IDictionary<string, string> arguments)
        {
            IActionFactory factory = provider?.GetActionFactory(actionName);

            if (factory == null)
            {
                logger.Warn($"No action factory for action: {actionName}.", arguments);
                return Task.CompletedTask;
            }

            IAction action = CreateActionInstance(factory);

            if (action == null)
            {
                logger.Warn($"No action created for action: {actionName}.", factory, arguments);
                return Task.CompletedTask;
            }

            return ExecuteActionAsync(action, arguments);
        }

        private async Task ExecuteActionAsync(IAction action, IDictionary<string, string> arguments)
        {
            try
            {
                await action.ExecuteAsync(arguments);
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
