using System;
using System.Runtime.Serialization;

namespace BeaverSoft.Texo.Core.Console.Decoding
{
    [Serializable]
    public class InvalidCommandException : InvalidByteException
    {
        public InvalidCommandException(byte command, string parameter)
           : base(command, $"Invalid command {command:X2} '{(char)command}' or parameter = '{parameter}'.")
        {
            Parameter = parameter;
        }

        protected InvalidCommandException(SerializationInfo info, StreamingContext context)
           : base(info, context)
        {
            info.AddValue(nameof(Parameter), Parameter);
        }

        public byte Command => Byte;

        public string Parameter { get; }
    }
}
