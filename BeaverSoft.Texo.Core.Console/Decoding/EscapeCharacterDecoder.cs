using System;
using System.Text;

namespace BeaverSoft.Texo.Core.Console.Decoding
{
    public abstract class EscapeCharacterDecoder : IDecoder
    {
        public const byte EscapeCharacter = 0x1B;
        public const byte BelCharacter = 7;
        public const byte LeftBracketCharacter = 0x5B;
        public const byte RightBracketCharacter = 0x5D;

        protected Encoding encoding;
        protected Decoder decoder;
        //protected Encoder encoder;
        private byte[] inputBuffer;

        public virtual event Action<IDecoder, byte[]> Output;

        public EscapeCharacterDecoder()
            : this(Encoding.Default) { }

        public EscapeCharacterDecoder(Encoding encoding)
        {
            SetEncoding(encoding ?? throw new ArgumentNullException(nameof(encoding)));
        }

        ~EscapeCharacterDecoder()
        {
            Dispose(false);
        }

        public Encoding Encoding
        {
            get => encoding;
            set => SetEncoding(value);
        }

        public void Input(byte[] data)
        {
            if (data.Length == 0)
            {
                throw new ArgumentException("Input can not process an empty array.");
            }

            if (inputBuffer != null)
            {
                byte[] temp = inputBuffer;
                inputBuffer = new byte[temp.Length + data.Length];
                Array.Copy(temp, 0, inputBuffer, 0, temp.Length);
                Array.Copy(data, 0, inputBuffer, temp.Length, data.Length);

                data = inputBuffer;
                inputBuffer = null;
            }

            int index = 0;

            while (index < data.Length)
            {
                int newIndex = data[index] == EscapeCharacter
                    ? OnCommand(data, index)
                    : OnNormalInput(data, index);

                if (newIndex < 0)
                {
                    int bufferSize = data.Length - index;
                    inputBuffer = new byte[bufferSize];
                    Array.Copy(data, index, inputBuffer, 0, bufferSize);
                    return;
                }

                index = newIndex;
            }
        }

        public void CharacterTyped(char character)
        {
            byte[] data = encoding.GetBytes(new char[] { character });
            OnOutput(data);
        }

        public virtual bool KeyPressed(Keys modifiers, Keys key)
        {
            return false;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            // no operation ( template method )
        }

        protected abstract void OnCharacters(char[] characters);
        protected abstract void ProcessCommand(byte commandGroup, byte command, string parameter);

        protected virtual bool IsValidOneCharacterCommand(byte command)
        {
            return false;
        }

        protected virtual bool IsValidParameterCharacter(byte commandGroup, byte character)
        {
            if (commandGroup == RightBracketCharacter) // OSC: Operating System Command
            {
                return character != BelCharacter
                    && character != EscapeCharacter;
            }

            return char.IsNumber((char)character)
                || character == ':'
                || character == ';'
                || character == '<'
                || character == '='
                || character == '>'
                || character == '"'
                || character == '?';
        }

        protected virtual void OnOutput(byte[] output)
        {
            Output?.Invoke(this, output);
        }

        protected void SetEncoding(Encoding encoding)
        {
            if (encoding == null)
            {
                return;
            }

            this.encoding = encoding;
            decoder = encoding.GetDecoder();
            //encoder = encoding.GetEncoder();
        }

        private int OnCommand(byte[] data, int startIndex)
        {
            if (data.Length <= startIndex + 1)
            {
                // More data needed
                return -1;
            }

            byte commandGroup = data[startIndex + 1];
            int start = startIndex + 1;

            if (commandGroup == LeftBracketCharacter      // CSI: Control Sequence Introducer '['
                || commandGroup == RightBracketCharacter) // OSC: Operating System Command ']'
            {
                start++;

                if (data.Length < startIndex + 3)
                {
                    // More data needed
                    return -1;
                }
            }

            bool insideQuotes = false;
            int end = start;

            while (end < data.Length && (IsValidParameterCharacter(commandGroup, data[end]) || insideQuotes))
            {
                if (data[end] == '"') // TODO: not sure about this
                {
                    insideQuotes = !insideQuotes;
                }

                end++;
            }

            if (data.Length == 2 && IsValidOneCharacterCommand(data[start]))
            {
                end = data.Length - 1;
            }

            if (end == data.Length)
            {
                // More data needed
                return -1;
            }

            byte[] parameterData = new byte[end - start];

            for (int i = 0; i < parameterData.Length; i++)
            {
                parameterData[i] = data[start + i];
            }

            int parameterLength = decoder.GetCharCount(parameterData, 0, parameterData.Length);
            char[] parameterChars = new char[parameterLength];
            decoder.GetChars(parameterData, 0, parameterData.Length, parameterChars, 0);
            string parameter = new string(parameterChars);

            byte command = data[end];
            ProcessCommand(commandGroup, command, parameter);
            return end + 1;
        }

        private int OnNormalInput(byte[] data, int startIndex)
        {
            int inputLength = CalculateNormalInputLength(data, startIndex);

            if (inputLength <= 0)
            {
                return startIndex;
            }

            int charCount = decoder.GetCharCount(data, startIndex, inputLength);
            char[] characters = new char[charCount];
            decoder.GetChars(data, startIndex, inputLength, characters, 0);
            OnCharacters(characters);
            return startIndex + inputLength;
        }

        private int CalculateNormalInputLength(byte[] data, int startIndex)
        {
            for (int i = startIndex; i < data.Length; i++)
            {
                if (data[i] == EscapeCharacter)
                {
                    return i - startIndex;
                }
            }

            return data.Length - startIndex;
        }
    }
}
