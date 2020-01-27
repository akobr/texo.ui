namespace BeaverSoft.Texo.Fallback.PowerShell.Core.Messages
{
    public enum MessageType
    {
        RequestCommand = 0,
        RequestQuiteCommand,
        RequestIndependentCommand,
        RequestCancellation,
        RequestApplicationShutdown,

        CommandStarted,
        CommandFinished,
        CommandOutput,
        CommandCancelled,

        ApplicationShutdown,
    }
}
