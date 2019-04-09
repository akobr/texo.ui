using System.Collections.Generic;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Core.Logging
{
    public class LogAggregationService : ILogService
    {
        private readonly List<ILogService> register;

        public LogAggregationService()
        {
            register = new List<ILogService>();
        }

        public LogAggregationService(params ILogService[] services)
            : this()
        {
            foreach (ILogService service in services)
            {
                RegisterLogService(service);
            }
        }

        public void RegisterLogService(ILogService service)
        {
            register.Add(service);
        }

        public void DeregisterService(ILogService service)
        {
            register.Remove(service);
        }

        public void ProcessMessage(ILogMessage message)
        {
            message.PerformMessage(this);
        }

        public void Log(LogMessageLevelEnum level, string message, params object[] args)
        {
            foreach (ILogService service in register)
            {
                service.Log(level, message, args);
            }
        }
    }
}
