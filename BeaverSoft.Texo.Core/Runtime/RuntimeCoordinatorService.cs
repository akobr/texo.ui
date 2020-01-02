using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Help;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.Inputting.History;
using BeaverSoft.Texo.Core.Intellisense;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Core.Runtime
{
    // TODO: [P2] Split entire texo monster to multiple engines (+ add support for middlewares and pipelines)
    // TODO: [P2] Refactoring: Split render and transformation (IResultProcessingService)
    public class RuntimeCoordinatorService : IRuntimeCoordinatorService
    {
        private static Type taskType = typeof(Task);
        private static Type typedResultType = typeof(Result<>);
        private static Type typedTaskResultType = typeof(TaskResult<>);
        private static Type voidType = typeof(void);

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

        public async Task ProcessAsync(string input, CancellationToken cancellation = default)
        {
            Input inputModel;

            try
            {
                inputModel = await EvaluateInputAsync(input);
            }
            catch (Exception exception)
            {
                logger.Error("Error during evaluating of input.", exception);
#if !DEBUG
                return;
#else
                throw exception;
#endif
            }

            try
            {
                await ProcessInputAsync(inputModel, cancellation);
            }
            catch (Exception exception)
            {
                logger.Error("Error during processing input (execution).", exception);
#if !DEBUG
                RenderError(inputModel, exception.Message);
#else
                throw exception;
#endif
            }
        }

        public Task ExecuteActionAsync(string actionUrl)
        {
            return actionManagement.ExecuteAsync(actionUrl);
        }

        public Task ExecuteActionAsync(string actionName, IDictionary<string, string> arguments)
        {
            return actionManagement.ExecuteAsync(actionName, arguments);
        }

        public void Dispose()
        {
            view.Dispose();
            commandManagement.Dispose();
        }

        private async Task<Input> EvaluateInputAsync(string input)
        {
            Input inputModel = await Task.Run(() => evaluator.Evaluate(input));
            history?.Enqueue(inputModel);
            return inputModel;
        }

        private async Task ProcessInputAsync(Input inputModel, CancellationToken cancellation)
        {
            if (!inputModel.Context.IsValid)
            {
                if (fallback != null
                    && string.IsNullOrEmpty(inputModel.Context.Key))
                {
                    Task<ICommandResult> fallbackTask = fallback.FallbackAsync(inputModel, cancellation);
                    await RenderAsync(inputModel, await fallbackTask, cancellation);
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

            await ProcessContextAsync(inputModel, inputModel.Context, cancellation);
        }

        private Task ProcessContextAsync(Input input, CommandContext context, CancellationToken cancellation)
        {
            object command = commandManagement.BuildCommand(context.Key);
            context.CancellationToken = cancellation; // TODO: [P2] Should not to be here

            switch (command)
            {
                case ICommand typedCommand:
                    return ProcessTypedCommandAsync(typedCommand, context, input, cancellation);

                default:
                    return ProcessUnknownCommandAsync(command, context, input, cancellation);
            }
        }

        private async Task ProcessTypedCommandAsync(ICommand command, CommandContext context, Input input, CancellationToken cancellation)
        {
            ICommandResult commandResult = await Task.Run(() => command.Execute(context), cancellation);
            await RenderAsync(input, commandResult, cancellation);
        }

        private Task ProcessUnknownCommandAsync(object command, CommandContext context, Input input, CancellationToken cancellation)
        {
            Type commandType = command.GetType();
            MethodInfo executionMethod = commandType.GetMethod(nameof(ICommand.Execute), BindingFlags.Public);
            ParameterInfo[] parameters = executionMethod?.GetParameters();

            if (executionMethod == null
                || parameters.Length != 1
                || parameters[0].ParameterType != typeof(CommandContext))
            {

                logger.Error(
                    "Invalid command type, at least public method Execute(CommandContext) needs to be presented.",
                    context.Key, command.GetType().FullName, input.ParsedInput.RawInput);
                return Task.CompletedTask;
            }

            return RenderAsync(
                input,
                TransformToResult(
                    InvokeCommandMethod(executionMethod, command, context),
                    executionMethod.ReturnType),
                cancellation);
        }

        private async Task RenderAsync(Input input, ICommandResult result, CancellationToken cancellation)
        {
            Render(input, await resultProcessing.TransfortAsync(result, cancellation));
        }

        private void Render(Input input, IImmutableList<IItem> items)
        {
            view.Render(input, items);
        }

        private void RenderError(Input input, string message)
        {
            Render(input, ImmutableList<IItem>.Empty.Add(Item.AsMarkdown($"> {message}")));
        }

        private static ICommandResult TransformToResult(object value, Type type)
        {
            if (type == voidType)
            {
                return new VoidResult();
            }

            if (!taskType.IsAssignableFrom(type))
            {
                Type targetResultType = typedResultType.MakeGenericType(type);
                return (ICommandResult)Activator.CreateInstance(targetResultType, value);
            }

            if (!type.IsGenericType
                || type.GetGenericArguments().Length != 1)
            {
                return new TaskResult((Task)value);
            }

            Type targetTaskResultType = typedTaskResultType.MakeGenericType(type);
            return (ICommandResult)Activator.CreateInstance(targetTaskResultType, value);
        }

        private static object InvokeCommandMethod(MethodInfo method, object command, CommandContext context)
        {
            return method.IsStatic
                ? method.Invoke(null, new object[] { context })
                : method.Invoke(command, new object[] { context });
        }
    }
}
