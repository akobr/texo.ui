using System;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.NamedPipes
{
    public class RawMessage
    {
        private readonly IMessageEncoding encoding;

        public RawMessage(ReadOnlyMemory<byte> bytes, IMessageEncoding encoding)
        {
            Data = bytes;
            this.encoding = encoding;
        }

        public ReadOnlyMemory<byte> Data { get; }

        public string AsString()
        {
            return encoding.Decode(Data.Span);
        }

        public TMessage AsType<TMessage>()
        {
            return encoding.Decode<TMessage>(Data.Span);
        }
    }
}
