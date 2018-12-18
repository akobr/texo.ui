using System;
using System.Collections.Generic;
using StrongBeaver.Core.Services.Logging;
using StrongBeaver.Core.Services.Reflection;

namespace BeaverSoft.Texo.Core.Actions
{
    // TODO: try catch retrieve, creation and execution
    public class ActionManagementService : IActionManagementService
    {
        private readonly IActionTypeProvider provider;
        private readonly IInstanceCreationService creation;
        private readonly IActionUrlParser parser;
        private readonly ILogService logger;
        private readonly Dictionary<string, Type> registered;

        public ActionManagementService(
            IActionTypeProvider provider,
            IInstanceCreationService creation,
            ILogService logger)
        {
            registered = new Dictionary<string, Type>();
            parser = new ActionUrlParser();

            this.provider = provider;
            this.creation = creation;
            this.logger = logger;
        }

        public void Register<TAction>(string actionName)
            where TAction : IAction
        {
            registered[actionName] = typeof(TAction);
        }

        public void Unregister(string actionName)
        {
            registered.Remove(actionName);
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
            Type actionType = TryGetActionType(actionName);

            if (actionType == null)
            {
                logger.Warn($"No action type for action name: {actionName}", arguments);
                return;
            }

            IAction action = creation.CreateInstance(actionType) as IAction;

            if (action == null)
            {
                logger.Warn($"No action created for action name: {actionName}", actionType, arguments);
                return;
            }

            action.Execute(arguments);
        }

        private Type TryGetActionType(string actionName)
        {
            if (registered.TryGetValue(actionName, out Type actionType))
            {
                return actionType;
            }

            return provider?.GetActionType(actionName);
        }
    }
}
