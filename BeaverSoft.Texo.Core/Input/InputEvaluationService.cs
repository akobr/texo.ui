﻿using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.InputTree;
using BeaverSoft.Texo.Core.Model.Configuration;
using BeaverSoft.Texo.Core.Services;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Core.Input
{
    public class InputEvaluationService : IInputEvaluationService
    {
        private readonly IInputParseService parser;
        private readonly IEnvironmentService environment;
        private readonly ISettingsService settings;
        private readonly ILogService logger;

        private ITextumConfiguration configuration;
        private InputTree.InputTree tree;

        public InputEvaluationService(
            IInputParseService parser,
            IEnvironmentService environment,
            ISettingsService settings,
            ILogService logger)
        {
            this.parser = parser;
            this.environment = environment;
            this.settings = settings;
            this.logger = logger;
        }

        public void Initialise()
        {
            configuration = settings.Configuration;

            IInputTreeBuilder treeBuilder = new InputTreeBuilder(logger);
            tree = treeBuilder.Build(configuration.Runtime.Commands, configuration.Runtime.DefaultCommand);
        }

        public IInput Evaluate(string input)
        {
            IParsedInput parsedInput = parser.Parse(input);

            IInputTreeEvaluationStrategy evaluation = new InputTreeEvaluationStrategy(tree, environment);
            return evaluation.Evaluate(parsedInput);
        }
    }
}
