using System.Diagnostics;
using BeaverSoft.Texo.Core.Logging;
using StrongBeaver.Core.Container;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.Startup
{
    public static class LoggingConfig
    {
        public static void ConfigureLogging(this SimpleIoc container)
        {
            container.Register<ILogService>(() => new LogAggregationService(
#if DEBUG
                new DebugLogService(),
#endif
                new UserAppDataLogService(LogMessageLevelEnum.Trace, "fallback-")));

            ConfigureConsoleDebugOutput();
        }

        [Conditional("DEBUG")]
        private static void ConfigureConsoleDebugOutput()
        {
            ConsoleTraceListener consoleTrace = new ConsoleTraceListener();
            Trace.Listeners.Add(consoleTrace);
        }
    }
}
