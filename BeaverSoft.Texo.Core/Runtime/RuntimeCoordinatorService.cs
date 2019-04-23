using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Actions;
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
        private readonly IActionManagementService actionManagement;
        private readonly IInputHistoryService history;
        private readonly IFallbackService fallback;

        public RuntimeCoordinatorService(
            IEnvironmentService environment,
            IInputEvaluationService evaluator,
            ICommandManagementService commandManagement,
            IResultProcessingService resultProcessing,
            IViewService view,
            IActionManagementService actionManagement,
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
            this.actionManagement = actionManagement ?? throw new ArgumentNullException(nameof(actionManagement));

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

        public async Task ProcessAsync(string input)
        {
            Input.Input inputModel = evaluator.Evaluate(input);
            history?.Enqueue(inputModel);

            if (!inputModel.Context.IsValid)
            {
                if (fallback != null
                    && string.IsNullOrEmpty(inputModel.Context.Key))
                {
                    Render(inputModel, await fallback.FallbackAsync(inputModel));
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

            await ProcessContextAsync(inputModel, inputModel.Context);
        }

        public void ExecuteAction(string actionUrl)
        {
            actionManagement.Execute(actionUrl);
        }

        public void ExecuteAction(string actionName, IDictionary<string, string> arguments)
        {
            actionManagement.Execute(actionName, arguments);
        }

        public void Dispose()
        {
            view.Dispose();
            commandManagement.Dispose();
        }

        private Task ProcessContextAsync(Input.Input input, CommandContext context)
        {
            ICommand command = commandManagement.BuildCommand(context.Key);

            switch (command)
            {
                case IAsyncCommand asyncCommand:
                    return ProcessAsyncCommand(asyncCommand, context, input);

                default:
                    return ProcessCommand(command, context, input);
            }
        }

        private async Task ProcessCommand(ICommand command, CommandContext context, Input.Input input)
        {
#if !DEBUG
            try
            {
#endif
                await ProcessCommandAsTask(command, context, input);
#if !DEBUG
            }
            catch (Exception exception)
            {
                RenderError(input, exception.Message);
            }
#endif
        }

        private async Task ProcessCommandAsTask(ICommand command, CommandContext context, Input.Input input)
        {
            ICommandResult result = await Task.Run(() => command.Execute(context)).ConfigureAwait(true);
            Render(input, result);
        }

        private async Task ProcessAsyncCommand(IAsyncCommand command, CommandContext context, Input.Input input)
        {
#if !DEBUG
            try
            {
#endif
                ICommandResult result = await command.ExecuteAsync(context);
                Render(input, result);
#if !DEBUG
            }
            catch (Exception exception)
            {
                RenderError(input, exception.Message);
            }
#endif
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
            Render(input, ImmutableList<IItem>.Empty.Add(Item.Markdown($"> {message}")));
        }
    }
}
