using System;
using System.Runtime.Serialization;

namespace BeaverSoft.Texo.Core.Console.Decoding
{
    [Serializable]
    public class InvalidByteException : Exception
    {
        public InvalidByteException(byte data, string message)
           : base(message)
        {
            Byte = data;
        }

        protected InvalidByteException(SerializationInfo info, StreamingContext context)
           : base(info, context)
        {
            info.AddValue(nameof(Byte), Byte);
        }

        public byte Byte { get; }
    }
}
