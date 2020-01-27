using System;

namespace BeaverSoft.Texo.Core.Console.Decoding
{
    [Serializable]
    public class DecodeException : Exception
    {
        public DecodeException() { }

        public DecodeException(string message) : base(message) { }

        public DecodeException(string message, Exception inner) : base(message, inner) { }

        protected DecodeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
