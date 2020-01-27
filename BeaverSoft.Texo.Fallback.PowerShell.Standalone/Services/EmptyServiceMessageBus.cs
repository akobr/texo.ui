using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.Services
{
    public class EmptyServiceMessageBus : IServiceMessageBus
    {
        public void Send<TMessage>(TMessage message)
            where TMessage : IServiceMessage { }

        public void Send<TMessage, TTarget>(TMessage message)
            where TMessage : IServiceMessage
            where TTarget : IMessageBusService<TMessage> { }

        public void Send<TMessage>(TMessage message, object token)
            where TMessage : IServiceMessage { }
    }
}
