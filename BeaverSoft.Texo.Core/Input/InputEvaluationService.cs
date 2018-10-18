using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.InputTree;
using BeaverSoft.Texo.Core.Model.Configuration;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Core.Input
{
    public class InputEvaluationService : IInputEvaluationService
    {
        private readonly IInputParseService parser;
        private readonly IEnvironmentService environment;
        private readonly ISettingService setting;
        private readonly ILogService logger;

        private ITextumConfiguration configuration;
        private InputTree.InputTree tree;

        public InputEvaluationService(
            IInputParseService parser,
            IEnvironmentService environment,
            ISettingService setting,
            ILogService logger)
        {
            this.parser = parser;
            this.environment = environment;
            this.setting = setting;
            this.logger = logger;
        }

        public void Initialise()
        {
            configuration = setting.Configuration;

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
