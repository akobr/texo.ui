using System;
using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Help;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Runtime
{
    public class RuntimeCoordinatorService : IRuntimeCoordinatorService
    {
        private readonly IEnvironmentService environment;
        private readonly IInputEvaluationService evaluator;
        private readonly ICommandManagementService commandManagement;
        private readonly IDidYouMeanService didYouMean;
        private readonly IResultProcessingService resultProcessing;
        private readonly IViewService view;

        private ImmutableList<string> previousCommands;

        public RuntimeCoordinatorService(
            IEnvironmentService environment,
            IInputEvaluationService evaluator,
            ICommandManagementService commandManagement,
            IResultProcessingService resultProcessing,
            IViewService view,
            IDidYouMeanService didYouMean)
        {
            this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
            this.evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
            this.commandManagement = commandManagement ?? throw new ArgumentNullException(nameof(commandManagement));
            this.resultProcessing = resultProcessing ?? throw new ArgumentNullException(nameof(resultProcessing));
            this.view = view ?? throw new ArgumentNullException(nameof(view));
            this.didYouMean = didYouMean;

            previousCommands = ImmutableList<string>.Empty;
        }

        public void Initialise()
        {
            environment.Initialise();
            evaluator.Initialise();
            view.Initialise(this);
        }

        public void Start()
        {
            view.Start();
        }

        public Input.Input PreProcess(string input)
        {
            Input.Input inputModel = evaluator.Evaluate(input);
            view.Render(inputModel);
            return inputModel;
        }

        public void Process(string input)
        {
            Input.Input inputModel = evaluator.Evaluate(input);

            if (!inputModel.Context.IsValid)
            {
                if (didYouMean == null)
                {
                    RenderError("Unknown input.");
                    return;
                }

                Render(didYouMean.Help(inputModel));
                return;
            }

            if (!commandManagement.HasCommand(inputModel.Context.Key))
            {
                RenderError($"The command is not registered: {inputModel.Context.Key}.");
                return;
            }

            previousCommands = previousCommands.Add(inputModel.ParsedInput.RawInput);
            ProcessContext(inputModel.Context);
        }

        public IImmutableList<string> GetPreviousCommands()
        {
            return previousCommands;
        }

        public void Dispose()
        {
            view.Dispose();
            commandManagement.Dispose();
        }

        private void ProcessContext(CommandContext context)
        {
            ICommand command = commandManagement.BuildCommand(context.Key);

            switch (command)
            {
                case IAsyncCommand asyncCommand:
                    ProcessAsyncCommand(asyncCommand, context);
                    break;

                default:
                    ProcessCommand(command, context);
                    break;
            }
        }

        private void ProcessCommand(ICommand command, CommandContext context)
        {
            try
            {
                ICommandResult result = command.Execute(context);
                Render(result);
            }
            catch (Exception exception)
            {
                RenderError(exception.Message);
            }
        }

        private async void ProcessAsyncCommand(IAsyncCommand command, CommandContext context)
        {
            try
            {
                ICommandResult result = await command.ExecuteAsync(context);
                Render(result);
            }
            catch (Exception exception)
            {
                RenderError(exception.Message);
            }
        }

        private void Render(ICommandResult result)
        {
            Render(resultProcessing.Transfort(result));
        }

        private void Render(IImmutableList<IItem> items)
        {
            view.Render(items);
        }

        private void RenderError(string message)
        {
            Render(ImmutableList<IItem>.Empty.Add(new Item($"> {message}")));
        }
    }
}
