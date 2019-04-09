using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Core.Extensions
{
    public static class LogServiceExtensions
    {
        public static void CommandError(this ILogService service, string commandKey, params object[] args)
        {
            service?.Log(LogMessageLevelEnum.Error, $"An error occured in {commandKey} command.", args);
        }
    }
}
