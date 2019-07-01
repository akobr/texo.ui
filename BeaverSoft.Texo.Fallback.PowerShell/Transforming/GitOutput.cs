using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Fallback.PowerShell.Transforming
{
    public class GitOutput : BaseCommandOutput
    {
        public GitOutput(ILogService logger)
            : base(logger)
        {
            Pipeline.AddPipe(new GitUpdateOutput());
            Pipeline.AddPipe(new GitStatusOutput());
            Pipeline.AddPipe(new GitBranchOutput());
            Pipeline.AddPipe(new GitPushOutput());
            Pipeline.AddPipe(new GitErrorUpdateOutput());
        }

        protected override bool IsInterestedOutput(OutputModel data)
        {
            return data.Flags.Contains(TransformationFlags.GIT);
        }
    }
}
