using System;
using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Help;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Input.History;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Runtime
{
    public class RuntimeCoordinatorService : IRuntimeCoordinatorService
    {
        private readonly IEnvironmentService environment;
        private readonly IInputEvaluationService evaluator;
        private readonly ICommandManagementService commandManagement;
        private readonly IDidYouMeanService didYouMean;
        private readonly IIntellisenceService intelisence;
        private readonly IResultProcessingService resultProcessing;
        private readonly IViewService view;
        private readonly IInputHistoryService history;
        private readonly IFallbackService fallback;

        public RuntimeCoordinatorService(
            IEnvironmentService environment,
            IInputEvaluationService evaluator,
            ICommandManagementService commandManagement,
            IResultProcessingService resultProcessing,
            IViewService view,
            IInputHistoryService history,
            IIntellisenceService intelisence,
            IDidYouMeanService didYouMean,
            IFallbackService fallback)
        {
            this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
            this.evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
            this.commandManagement = commandManagement ?? throw new ArgumentNullException(nameof(commandManagement));
            this.resultProcessing = resultProcessing ?? throw new ArgumentNullException(nameof(resultProcessing));
            this.view = view ?? throw new ArgumentNullException(nameof(view));

            this.history = history;
            this.intelisence = intelisence;
            this.didYouMean = didYouMean;
            this.fallback = fallback;
        }

        public void Initialise()
        {
            environment.Initialise();
            evaluator.Initialise();
            view.Initialise(this);
            fallback?.Initialise();
        }

        public void Start()
        {
            view.Start();
        }

        public Input.Input PreProcess(string input, int cursorPosition)
        {
            Input.Input inputModel = evaluator.Evaluate(input);
            var items = intelisence?.Help(inputModel, cursorPosition) ?? ImmutableList<IItem>.Empty;
            view.RenderIntellisence(inputModel, items);
            return inputModel;
        }

        public void Process(string input)
        {
            Input.Input inputModel = evaluator.Evaluate(input);
            history?.Enqueue(inputModel);

            if (!inputModel.Context.IsValid)
            {
                if (fallback != null
                    && string.IsNullOrEmpty(inputModel.Context.Key))
                {
                    Render(inputModel, fallback.Fallback(inputModel));
                }
                else if (didYouMean != null)
                {
                    Render(inputModel, didYouMean.Help(inputModel));
                }
                else
                {
                    RenderError(inputModel, "Unknown input.");
                }
                return;
            }

            if (!commandManagement.HasCommand(inputModel.Context.Key))
            {
                RenderError(inputModel, $"The command is not registered: {inputModel.Context.Key}.");
                return;
            }

            ProcessContext(inputModel, inputModel.Context);
        }

        public void Dispose()
        {
            view.Dispose();
            commandManagement.Dispose();
        }

        private void ProcessContext(Input.Input input, CommandContext context)
        {
            ICommand command = commandManagement.BuildCommand(context.Key);

            switch (command)
            {
                case IAsyncCommand asyncCommand:
                    ProcessAsyncCommand(asyncCommand, context, input);
                    break;

                default:
                    ProcessCommand(command, context, input);
                    break;
            }
        }

        private void ProcessCommand(ICommand command, CommandContext context, Input.Input input)
        {
            try
            {
                ICommandResult result = command.Execute(context);
                Render(input, result);
            }
            catch (Exception exception)
            {
                RenderError(input, exception.Message);
            }
        }

        private async void ProcessAsyncCommand(IAsyncCommand command, CommandContext context, Input.Input input)
        {
            try
            {
                ICommandResult result = await command.ExecuteAsync(context);
                Render(input, result);
            }
            catch (Exception exception)
            {
                RenderError(input, exception.Message);
            }
        }

        private void Render(Input.Input input, ICommandResult result)
        {
            Render(input, resultProcessing.Transfort(result));
        }

        private void Render(Input.Input input, IImmutableList<IItem> items)
        {
            view.Render(input, items);
        }

        private void RenderError(Input.Input input, string message)
        {
            Render(input, ImmutableList<IItem>.Empty.Add(new Item($"> {message}")));
        }
    }
}
