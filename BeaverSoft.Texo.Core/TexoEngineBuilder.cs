using System;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Help;
using BeaverSoft.Texo.Core.Input;
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

        private ILogService logger;
        private ISettingService setting;
        private IEnvironmentService environment;
        private IInputParseService parser;
        private IInputEvaluationService evaluator;
        private ITexoFactory<ICommand, string> commandFactory;
        private ICommandManagementService commandManagement;

        private IDidYouMeanService didYouMean;
        private IResultProcessingService resultProcessing;
        private IViewService usedView;
        private IRuntimeCoordinatorService runtime;

        public TexoEngineBuilder()
        {
            ServiceMessageBus defaultMessageBus = new ServiceMessageBus();
            messageBus = defaultMessageBus;
            registerToMessageBus = defaultMessageBus;
        }

        public TexoEngineBuilder(IServiceMessageBus messageBus, IServiceMessageBusRegister registerToMessageBus)
        {
            this.messageBus = messageBus;
            this.registerToMessageBus = registerToMessageBus;
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
            SetEnvironmentService(service);
            return this;
        }

        public TexoEngineBuilder WithInputParseService(IInputParseService service)
        {
            parser = service;
            return this;
        }

        public TexoEngineBuilder WithInputEvaluationService(IInputEvaluationService service)
        {
            SetInputEvaluationService(service);
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

        public TexoEngineBuilder WithDidYouMeanService(IDidYouMeanService service)
        {
            didYouMean = service;
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
                CommandManagement = () => commandManagement,
                ResultProcessing = () => resultProcessing,
                DidYouMean = () => didYouMean,
                View = () => usedView,
                Runtime = () => runtime
            };
        }

        public TexoEngine Build(ITexoFactory<ICommand, string> factory, IViewService view)
        {
            commandFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            Initiliase();
            SetViewService(view);
            runtime = new RuntimeCoordinatorService(environment, evaluator, commandManagement, resultProcessing, usedView, didYouMean);
            return new TexoEngine(runtime, usedView, setting);
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
            SetEnvironmentService(environment ?? new EnvironmentService(messageBus));
            parser = parser ?? new InputParseService();
            SetInputEvaluationService(evaluator ?? new InputEvaluationService(parser, environment, logger));
            commandManagement = commandManagement ?? new SingletonCommandManagementService(commandFactory);
            resultProcessing = resultProcessing ?? new ResultProcessingService(logger);
            // didYouMean
        }

        private void SetEnvironmentService(IEnvironmentService service)
        {
            if (environment != null)
            {
                registerToMessageBus.Unregister(environment);
            }

            environment = service;
            registerToMessageBus.Register<ISettingUpdatedMessage>(environment);
        }

        private void SetInputEvaluationService(IInputEvaluationService service)
        {
            if (evaluator != null)
            {
                registerToMessageBus.Unregister(evaluator);
            }

            evaluator = service;
            registerToMessageBus.Register<ISettingUpdatedMessage>(evaluator);
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
        }
    }
}