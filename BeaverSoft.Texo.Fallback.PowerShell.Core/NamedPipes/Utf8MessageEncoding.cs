using System;
using System.Text;
using System.Text.Json;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.NamedPipes
{
    class Utf8MessageEncoding : IMessageEncoding
    {
        private readonly JsonSerializerOptions serialiserOptions;

        public Utf8MessageEncoding(JsonSerializerOptions serialiserOptions = null)
        {
            this.serialiserOptions = serialiserOptions;
        }

        public string Decode(ReadOnlySpan<byte> rawMessage)
        {
            // TODO: [P3] Solve unnecesary re-type to array
            return Encoding.UTF8.GetString(rawMessage.ToArray());
        }

        public TMessage Decode<TMessage>(ReadOnlySpan<byte> rawMessage)
        {
            return JsonSerializer.Deserialize<TMessage>(rawMessage, serialiserOptions);
        }

        public byte[] Encode(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }

        public byte[] Encode<TMessage>(TMessage message)
        {
            return JsonSerializer.SerializeToUtf8Bytes(message, serialiserOptions);
        }
    }
}
