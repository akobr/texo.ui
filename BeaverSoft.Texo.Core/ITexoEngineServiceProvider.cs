using System;
using System.Collections.Generic;
using System.Text;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Help;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Input.History;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core.Services;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Core
{
    public interface ITexoEngineServiceProvider
    {
        IServiceMessageBus MessageBus { get; }

        IServiceMessageBusRegister MessageBusRegister { get; }

        ILogService Logger { get; }

        ISettingService Setting { get; }

        IEnvironmentService Environment { get; }

        IInputParseService Parser { get; }

        IInputEvaluationService Evaluator { get; }

        IInputHistoryService History { get; }

        ICommandManagementService CommandManagement { get; }

        IResultProcessingService ResultProcessing { get; }

        IIntellisenceService Intellisence { get; }

        IDidYouMeanService DidYouMean { get; }

        IFallbackService Fallback { get; }

        IViewService View { get; }

        IRuntimeCoordinatorService Runtime { get; }
    }
}
