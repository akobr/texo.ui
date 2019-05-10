using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Help;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.Inputting.History;
using BeaverSoft.Texo.Core.Intellisense;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Core.Runtime
{
    public class RuntimeCoordinatorService : IRuntimeCoordinatorService
    {
        private readonly IEnvironmentService environment;
        private readonly IInputEvaluationService evaluator;
        private readonly ICommandManagementService commandManagement;
        private readonly IDidYouMeanService didYouMean;
        private readonly IIntellisenseService intellisense;
        private readonly IResultProcessingService resultProcessing;
        private readonly IViewService view;
        private readonly IActionManagementService actionManagement;
        private readonly IInputHistoryService history;
        private readonly IFallbackService fallback;
        private readonly ILogService logger;

        public RuntimeCoordinatorService(
            IEnvironmentService environment,
            IInputEvaluationService evaluator,
            ICommandManagementService commandManagement,
            IResultProcessingService resultProcessing,
            IViewService view,
            IActionManagementService actionManagement,
            IInputHistoryService history,
            IIntellisenseService intellisense,
            IDidYouMeanService didYouMean,
            IFallbackService fallback,
            ILogService logger)
        {
            this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
            this.evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
            this.commandManagement = commandManagement ?? throw new ArgumentNullException(nameof(commandManagement));
            this.resultProcessing = resultProcessing ?? throw new ArgumentNullException(nameof(resultProcessing));
            this.view = view ?? throw new ArgumentNullException(nameof(view));
            this.actionManagement = actionManagement ?? throw new ArgumentNullException(nameof(actionManagement));

            this.logger = logger;
            this.history = history;
            this.intellisense = intellisense;
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

        public Input PreProcess(string input, int cursorPosition)
        {
            Input inputModel = evaluator.Evaluate(input);
            
            if (intellisense == null)
            {
                return inputModel;
            }

            intellisense
                .HelpAsync(inputModel, cursorPosition)
                .ContinueWith((task) =>
                    {
                        if (task.Status == TaskStatus.Faulted)
                        {
                            logger.Error("Retrieving of intellisense failed.", task.Exception);
                            return;
                        }

                        if (task.Result != null)
                        {
                            view.RenderIntellisense(inputModel, task.Result);
                        }
                    });

            return inputModel;
        }

        public Task ProcessAsync(string input)
        {
            try
            {
                return ProcessInputAsync(input);
            }
            catch (Exception exception)
            {
                logger.Error("Error during processing input (execution).", exception);
#if !DEBUG
                RenderError(input, exception.Message);
#else
                throw exception;
#endif
            }
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

        private async Task ProcessInputAsync(string input)
        {
            Input inputModel = evaluator.Evaluate(input);
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

        private Task ProcessContextAsync(Input input, CommandContext context)
        {
            ICommand command = commandManagement.BuildCommand(context.Key);

            switch (command)
            {
                case IAsyncCommand asyncCommand:
                    return ProcessAsyncCommand(asyncCommand, context, input);

                default:
                    return ProcessCommandAsTask(command, context, input);
            }
        }

        private async Task ProcessCommandAsTask(ICommand command, CommandContext context, Input input)
        {
            ICommandResult result = await Task.Run(() => command.Execute(context)).ConfigureAwait(true);
            Render(input, result);
        }

        private async Task ProcessAsyncCommand(IAsyncCommand command, CommandContext context, Input input)
        {
            ICommandResult result = await command.ExecuteAsync(context);
            Render(input, result);
        }

        private void Render(Input input, ICommandResult result)
        {
            Render(input, resultProcessing.Transfort(result));
        }

        private void Render(Input input, IImmutableList<IItem> items)
        {
            view.Render(input, items);
        }

        private void RenderError(Input input, string message)
        {
            Render(input, ImmutableList<IItem>.Empty.Add(Item.Markdown($"> {message}")));
        }
    }
}
