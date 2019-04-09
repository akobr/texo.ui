using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Input.InputTree;
using BeaverSoft.Texo.Core.Runtime;
using StrongBeaver.Core.Messaging;
using StrongBeaver.Core.Services;
using StrongBeaver.Core.Services.Logging;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Input
{
    public class InputEvaluationService : IInputEvaluationService, IMessageBusService<ISettingUpdatedMessage>
    {
        private readonly IInputParseService parser;
        private readonly IEnvironmentService environment;
        private readonly IServiceMessageBus messageBus;
        private readonly ILogService logger;

        private TextumConfiguration configuration;
        private InputTree.InputTree tree;

        public InputEvaluationService(
            IInputParseService parser,
            IEnvironmentService environment,
            IServiceMessageBus messageBus,
            ILogService logger)
        {
            this.parser = parser;
            this.environment = environment;
            this.messageBus = messageBus;
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

        async void IMessageBusRecipient<ISettingUpdatedMessage>.ProcessMessage(ISettingUpdatedMessage message)
        {
            try
            {
                configuration = message.Configuration;
                tree = await Task.Run(() => BuildInputTree(message.Configuration));

                if (configuration != message.Configuration)
                {
                    return;
                }

                messageBus.Send<IInputTreeUpdatedMessage>(new InputTreeUpdatedMessage(configuration, tree));
            }
            catch (System.Exception exception)
            {
                logger.Error("An unknown error happend during input tree build.", exception);
            }            
        }

        private InputTree.InputTree BuildInputTree(TextumConfiguration fromConfiguration)
        {
            IInputTreeBuilder treeBuilder = new InputTreeBuilder(logger);
            return treeBuilder.Build(fromConfiguration.Runtime.Commands, fromConfiguration.Runtime.DefaultCommand);
        }
    }
}
