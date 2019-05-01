using System;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Help;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.Inputting.History;
using BeaverSoft.Texo.Core.Intellisense;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core.Services;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Core
{
    public class TexoEngineServiceLocator : ITexoEngineServiceLocator
    {
        public Func<IServiceMessageBus> MessageBus { get; set; }

        public Func<IServiceMessageBusRegister> MessageBusRegister { get; set; }

        public Func<ILogService> Logger { get; set; }

        public Func<ISettingService> Setting { get; set; }

        public Func<IEnvironmentService> Environment { get; set; }

        public Func<IInputParseService> Parser { get; set; }

        public Func<IInputEvaluationService> Evaluator { get; set; }

        public Func<IInputHistoryService> History { get; set; }

        public Func<ICommandManagementService> CommandManagement { get; set; }

        public Func<IResultProcessingService> ResultProcessing { get; set; }

        public Func<IIntellisenseService> Intellisense { get; set; }

        public Func<IDidYouMeanService> DidYouMean { get; set; }

        public Func<IFallbackService> Fallback { get; set; }

        public Func<IViewService> View { get; set; }

        public Func<IActionManagementService> ActionManagement { get; set; }

        public Func<IRuntimeCoordinatorService> Runtime { get; set; }

        public Func<IActionFactoryProvider> ActionProvider { get; set; }

        public Func<IActionFactoryRegister> ActionRegister { get; set; }
    }
}
