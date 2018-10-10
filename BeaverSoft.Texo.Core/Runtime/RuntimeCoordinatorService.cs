using System;
using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Model.View;
using BeaverSoft.Texo.Core.Services;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Runtime
{
    public class RuntimeCoordinatorService : IRuntimeCoordinatorService
    {
        private readonly IInputParseService parser;
        private readonly ICommandContextBuildService contextBuilder;
        private readonly ICommandManagementService commandManagement;
        private readonly IDidYouMeanService didYouMean;
        private readonly IResultProcessingService resultProcessing;
        private readonly IViewService view;

        public RuntimeCoordinatorService(
            IInputParseService parser,
            ICommandContextBuildService contextBuilder,
            ICommandManagementService commandManagement,
            IDidYouMeanService didYouMean,
            IResultProcessingService resultProcessing,
            IViewService view)
        {
            this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
            this.contextBuilder = contextBuilder ?? throw new ArgumentNullException(nameof(contextBuilder));
            this.commandManagement = commandManagement ?? throw new ArgumentNullException(nameof(commandManagement));
            this.didYouMean = didYouMean ?? throw new ArgumentNullException(nameof(didYouMean));
            this.resultProcessing = resultProcessing ?? throw new ArgumentNullException(nameof(resultProcessing));
            this.view = view ?? throw new ArgumentNullException(nameof(view));
        }

        public void Process(string input)
        {
            IParsedInput parsedInput = parser.Parse(input);

            if (parsedInput.IsEmpty())
            {
                return;
            }

            ICommandContext context = contextBuilder.BuildContext(parsedInput);

            if (context.IsValid)
            {
                Render(didYouMean.Help(parsedInput));
                return;
            }

            if (!commandManagement.HasCommand(context.Key))
            {
                RenderError("Invalid command!");
                return;
            }

            ProcessContext(context);
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
