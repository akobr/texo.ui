using System;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.NamedPipes
{
    public interface IMessageEncoding
    {
        byte[] Encode(string message);

        byte[] Encode<TMessage>(TMessage message);

        string Decode(ReadOnlySpan<byte> rawMessage);

        TMessage Decode<TMessage>(ReadOnlySpan<byte> rawMessage);
    }
}
