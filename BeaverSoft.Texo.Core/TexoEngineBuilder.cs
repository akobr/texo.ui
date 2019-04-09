using System;
using System.Threading;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Help;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core;
using StrongBeaver.Core.Services;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Core
{
    public class TexoEngineBuilder : IFluentSyntax
    {
        private readonly ServiceMessageBus messageBus;

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
            : this(new ServiceMessageBus())
        {
            // no member
        }

        public TexoEngineBuilder(ServiceMessageBus messageBus)
        {
            this.messageBus = messageBus;
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

        public TexoEngineBuilder WithCommandFactory(ITexoFactory<ICommand, string> factory)
        {
            commandFactory = factory;
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

        public TexoEngine Build(IViewService view)
        {
            Initiliase();
            SetViewService(view);
            runtime = new RuntimeCoordinatorService(environment, evaluator, commandManagement, resultProcessing, usedView, didYouMean);
            return new TexoEngine(runtime, setting);
        }

        private void Initiliase()
        {
#if DEBUG
            logger = logger ?? new DebugLogService();
#else
            logger = logger ?? new EmptyLogService();
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
                messageBus.Unregister(environment);
            }

            environment = service;
            messageBus.Register<IEnvironmentService, ISettingUpdatedMessage>(environment);
        }

        private void SetInputEvaluationService(IInputEvaluationService service)
        {
            if (evaluator != null)
            {
                messageBus.Unregister(evaluator);
            }

            evaluator = service;
            messageBus.Register<IInputEvaluationService, ISettingUpdatedMessage>(evaluator);
        }

        private void SetViewService(IViewService service)
        {
            if (usedView != null)
            {
                messageBus.Unregister(usedView);
            }

            usedView = service;
            messageBus.Register<IViewService, ISettingUpdatedMessage>(usedView);
            messageBus.Register<IViewService, IVariableUpdatedMessage>(usedView);
        }
    }
}
