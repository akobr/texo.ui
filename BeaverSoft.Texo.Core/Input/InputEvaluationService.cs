using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Input.InputTree;
using StrongBeaver.Core.Messaging;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Core.Input
{
    public class InputEvaluationService : IInputEvaluationService
    {
        private readonly IInputParseService parser;
        private readonly IEnvironmentService environment;
        private readonly ILogService logger;

        private TextumConfiguration configuration;
        private InputTree.InputTree tree;

        public InputEvaluationService(
            IInputParseService parser,
            IEnvironmentService environment,
            ILogService logger)
        {
            this.parser = parser;
            this.environment = environment;
            this.logger = logger;
        }

        public void Initialise()
        {
            // no operation
        }

        public Input Evaluate(string input)
        {
            ParsedInput parsedInput = parser.Parse(input);

            IInputTreeEvaluationStrategy evaluation = new InputTreeEvaluationStrategy(tree, environment);
            return evaluation.Evaluate(parsedInput);
        }

        void IMessageBusRecipient<ISettingUpdatedMessage>.ProcessMessage(ISettingUpdatedMessage message)
        {
            if (configuration != null)
            {
                return;
            }

            configuration = message.Configuration;

            IInputTreeBuilder treeBuilder = new InputTreeBuilder(logger);
            tree = treeBuilder.Build(configuration.Runtime.Commands, configuration.Runtime.DefaultCommand);
        }
    }
}
