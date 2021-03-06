using System;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Actions.Implementations;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Help;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.Inputting.History;
using BeaverSoft.Texo.Core.Intellisense;
using BeaverSoft.Texo.Core.Logging;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core;
using StrongBeaver.Core.Services;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Core
{
    public class TexoEngineBuilder : IFluentSyntax
    {
        private readonly IServiceMessageBus messageBus;
        private readonly IServiceMessageBusRegister registerToMessageBus;
        private readonly ActionFactories actions;

        private ILogService logger;
        private ISettingService setting;
        private IEnvironmentService environment;
        private IInputParseService parser;
        private IInputEvaluationService evaluator;
        private IInputHistoryService history;

        private ITexoFactory<object, string> commandFactory;
        private ICommandManagementService commandManagement;

        private IFallbackService fallback;
        private IIntellisenseService intellisense;
        private IDidYouMeanService didYouMean;
        private IResultProcessingService resultProcessing;
        private IViewService usedView;
        private IActionManagementService actionManagement;
        private IRuntimeCoordinatorService runtime;

        public TexoEngineBuilder()
        {
            ServiceMessageBus defaultMessageBus = new ServiceMessageBus();
            messageBus = defaultMessageBus;
            registerToMessageBus = defaultMessageBus;
            actions = new ActionFactories();
        }

        public TexoEngineBuilder(IServiceMessageBus messageBus, IServiceMessageBusRegister registerToMessageBus)
        {
            this.messageBus = messageBus;
            this.registerToMessageBus = registerToMessageBus;
            actions = new ActionFactories();
        }

        public TexoEngineBuilder WithLogService(ILogService service)
        {
            logger = service;
            return this;
        }

        public TexoEngineBuilder WithSettingService(ISettingService service)
        {
            setting = service;
            return this;
        }

        public TexoEngineBuilder WithEnvironmentService(IEnvironmentService service)
        {
            environment = service;
            return this;
        }

        public TexoEngineBuilder WithInputParseService(IInputParseService service)
        {
            parser = service;
            return this;
        }

        public TexoEngineBuilder WithInputEvaluationService(IInputEvaluationService service)
        {
            evaluator = service;
            return this;
        }

        public TexoEngineBuilder WithInputHistoryService(IInputHistoryService service)
        {
            history = service;
            return this;
        }

        public TexoEngineBuilder WithCommandManagementService(ICommandManagementService service)
        {
            commandManagement = service;
            return this;
        }

        public TexoEngineBuilder WithResultProcessingService(IResultProcessingService service)
        {
            resultProcessing = service;
            return this;
        }

        public TexoEngineBuilder WithActionManagementService(IActionManagementService service)
        {
            actionManagement = service;
            return this;
        }

        public TexoEngineBuilder WithIntellisenseService(IIntellisenseService service)
        {
            intellisense = service;
            return this;
        }

        public TexoEngineBuilder WithDidYouMeanService(IDidYouMeanService service)
        {
            didYouMean = service;
            return this;
        }

        public TexoEngineBuilder WithFallbackService(IFallbackService service)
        {
            fallback = service;
            return this;
        }

        public ITexoEngineServiceLocator GetServiceLocator()
        {
            return new TexoEngineServiceLocator
            {
                MessageBus = () => messageBus,
                MessageBusRegister = () => registerToMessageBus,
                Logger = () => logger,
                Setting = () => setting,
                Environment = () => environment,
                Parser = () => parser,
                Evaluator = () => evaluator,
                History = () => history,
                CommandManagement = () => commandManagement,
                ResultProcessing = () => resultProcessing,
                Intellisense = () => intellisense,
                DidYouMean = () => didYouMean,
                Fallback = () => fallback,
                View = () => usedView,
                ActionManagement = () => actionManagement,
                Runtime = () => runtime,
                ActionRegister = () => actions,
                ActionProvider = () => actions,
            };
        }

        public TexoEngine Build(ITexoFactory<object, string> factory, IViewService view)
        {
            commandFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            Initiliase();
            SetViewService(view);
            runtime = new RuntimeCoordinatorService(
                environment, evaluator, commandManagement,
                resultProcessing, usedView, actionManagement,
                history, intellisense, didYouMean, fallback, logger);
            InitialiseActions();
            return new TexoEngine(runtime, usedView, actions, setting, logger);
        }

        private void Initiliase()
        {
#if DEBUG
            logger = logger ?? new LogAggregationService(
                         new DebugLogService(),
                         new UserAppDataLogService(LogMessageLevelEnum.Trace));
#else
            logger = logger ?? new UserAppDataLogService(LogMessageLevelEnum.Error);
#endif
            setting = setting ?? new SettingService(messageBus);
            environment = environment ?? CreateEnvironmentService();
            parser = parser ?? new InputParseService();
            evaluator = evaluator ?? CreateInputEvaluationService();
            history = history ?? new InputHistoryService();
            commandManagement = commandManagement ?? new SingletonCommandManagementService(commandFactory);
            resultProcessing = resultProcessing ?? new ResultProcessingService(logger);
            actionManagement = actionManagement ?? new ActionManagementService(actions, logger);
            intellisense = intellisense ?? CreateIntellisenseService();
            // didYouMean
        }

        private void InitialiseActions()
        {
            actions.RegisterFactory(new CommandRunActionFactory(runtime), ActionNames.COMMAND_RUN, ActionNames.COMMAND);
        }

        private IEnvironmentService CreateEnvironmentService()
        {
            var service = new EnvironmentService(messageBus);
            registerToMessageBus.Register<ISettingUpdatedMessage>(service);
            return service;
        }

        private IInputEvaluationService CreateInputEvaluationService()
        {
            var service = new InputEvaluationService(parser, environment, messageBus, logger);
            registerToMessageBus.Register<ISettingUpdatedMessage>(service);
            return service;
        }

        private IntellisenseService CreateIntellisenseService()
        {
            var service = new IntellisenseService(environment, commandManagement);
            registerToMessageBus.Register<IInputTreeUpdatedMessage>(service);
            return service;
        }

        private void SetViewService(IViewService service)
        {
            if (usedView != null)
            {
                registerToMessageBus.Unregister(usedView);
            }

            usedView = service ?? throw new ArgumentNullException(nameof(service));
            registerToMessageBus.Register<ISettingUpdatedMessage>(usedView);
            registerToMessageBus.Register<IVariableUpdatedMessage>(usedView);
            registerToMessageBus.Register<IClearViewOutputMessage>(usedView);
            registerToMessageBus.Register<PromptUpdateMessage>(usedView);
        }
    }
}