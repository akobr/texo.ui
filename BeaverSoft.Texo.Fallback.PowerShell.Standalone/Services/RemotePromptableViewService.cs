using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.Services
{
    public class RemotePromptableViewService : IPromptableViewService
    {
        private readonly ILogService logger;

        public RemotePromptableViewService(ILogService logger)
        {
            this.logger = logger;
        }

        public string GetNewInput()
        {
            logger.Debug("New input has been requested.");
            return string.Empty;
        }

        public void ShowProgress(int id, string name, int progress)
        {
            logger.Debug($"Progress report: {id}, {name}, {progress}.");
        }
    }
}
