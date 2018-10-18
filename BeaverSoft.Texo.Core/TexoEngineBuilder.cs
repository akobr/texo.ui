using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Help;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Core
{
    public class TexoEngineBuilder : IFluentSyntax
    {
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
            usedView = view;
            runtime = new RuntimeCoordinatorService(evaluator, commandManagement, resultProcessing, view, didYouMean);
            return new TexoEngine(runtime, setting);
        }

        private void Initiliase()
        {
#if DEBUG
            logger = logger ?? new DebugLogService();
#else
            logger = logger ?? new EmptyLogService();
#endif

            setting = setting ?? new SettingService();
            environment = environment ?? new EnvironmentService();
            parser = parser ?? new InputParseService();
            evaluator = evaluator ?? new InputEvaluationService(parser, environment, setting, logger);
            commandManagement = commandManagement ?? new SingletonCommandManagementService(commandFactory);
            // didYouMean
        }
    }
}
