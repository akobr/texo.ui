using System;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.Services
{
    public class ConsoleLogService : ILogService
    {
        public void Log(LogMessageLevelEnum level, string message, params object[] args)
        {
            Console.WriteLine($"[{level}] {message}");
        }

        public void ProcessMessage(ILogMessage message)
        {
            message.PerformMessage(this);
        }
    }
}
