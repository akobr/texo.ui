using System;
using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Help;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Model.View;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Runtime
{
    public class RuntimeCoordinatorService : IRuntimeCoordinatorService
    {
        private readonly IInputEvaluationService evaluator;
        private readonly ICommandManagementService commandManagement;
        private readonly IDidYouMeanService didYouMean;
        private readonly IResultProcessingService resultProcessing;
        private readonly IViewService view;

        public RuntimeCoordinatorService(
            IInputEvaluationService evaluator,
            ICommandManagementService commandManagement,
            IResultProcessingService resultProcessing,
            IViewService view,
            IDidYouMeanService didYouMean)
        {
            this.evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
            this.commandManagement = commandManagement ?? throw new ArgumentNullException(nameof(commandManagement));
            this.resultProcessing = resultProcessing ?? throw new ArgumentNullException(nameof(resultProcessing));
            this.view = view ?? throw new ArgumentNullException(nameof(view));
            this.didYouMean = didYouMean;
        }

        public void Process(string input)
        {
            IInput inputModel = evaluator.Evaluate(input);

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

            ProcessContext(inputModel.Context);
        }

        public void Dispose()
        {
            view.Dispose();
            commandManagement.Dispose();
        }

        private void ProcessContext(ICommandContext context)
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

        private void ProcessCommand(ICommand command, ICommandContext context)
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

        private async void ProcessAsyncCommand(IAsyncCommand command, ICommandContext context)
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
