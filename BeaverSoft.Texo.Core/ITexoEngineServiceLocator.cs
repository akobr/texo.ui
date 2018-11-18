﻿using System;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Help;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core.Services;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Core
{
    public interface ITexoEngineServiceLocator
    {
        Func<IServiceMessageBus> MessageBus { get; }

        Func<IServiceMessageBusRegister> MessageBusRegister { get; }

        Func<ILogService> Logger { get; }

        Func<ISettingService> Setting { get; }

        Func<IEnvironmentService> Environment { get; }

        Func<IInputParseService> Parser { get; }

        Func<IInputEvaluationService> Evaluator { get; }

        Func<ICommandManagementService> CommandManagement { get; }

        Func<IResultProcessingService> ResultProcessing { get; }

        Func<IDidYouMeanService> DidYouMean { get; }

        Func<IViewService> View { get; }

        Func<IRuntimeCoordinatorService> Runtime { get; }


    }
}
