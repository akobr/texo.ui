using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeaverSoft.Texo.Core.Console.Decoding
{
    internal abstract class EscapeCharacterDecoder : IDecoder
    {
        public const byte EscapeCharacter = 0x1B;
        public const byte LeftBracketCharacter = 0x5B;
        public const byte XonCharacter = 17;
        public const byte XoffCharacter = 19;

        private readonly List<byte> commandBuffer;
        protected readonly List<byte[]> outBuffer;

        protected DecoderState state;
        protected Encoding encoding;
        protected Decoder decoder;
        //protected Encoder encoder;

        protected bool supportXonXoff;
        protected bool xOffReceived;

        public virtual event Action<IDecoder, byte[]> Output;

        public EscapeCharacterDecoder()
            : this(Encoding.Default) { }

        public EscapeCharacterDecoder(Encoding encoding)
        {
            commandBuffer = new List<byte>();
            outBuffer = new List<byte[]>();

            state = DecoderState.Normal;
            SetEncoding(encoding);

            supportXonXoff = true;
            xOffReceived = false;
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

            if (supportXonXoff)
            {
                foreach (byte b in data)
                {
                    if (b == XoffCharacter)
                    {
                        xOffReceived = true;
                    }
                    else if (b == XonCharacter)
                    {
                        xOffReceived = false;
                        if (outBuffer.Count > 0)
                        {
                            foreach (byte[] output in outBuffer)
                            {
                                OnOutput(output);
                            }
                        }
                    }
                }
            }

            switch (state)
            {
                case DecoderState.Normal:
                    if (data[0] == EscapeCharacter)
                    {
                        AddToCommandBuffer(data);
                        ProcessCommandBuffer();
                    }
                    else
                    {
                        int i = 0;
                        while (i < data.Length && data[i] != EscapeCharacter)
                        {
                            ProcessNormalInput(data[i]);
                            i++;
                        }
                        if (i != data.Length)
                        {
                            while (i < data.Length)
                            {
                                AddToCommandBuffer(data[i]);
                                i++;
                            }
                            ProcessCommandBuffer();
                        }
                    }
                    break;

                case DecoderState.Command:
                    AddToCommandBuffer(data);
                    ProcessCommandBuffer();
                    break;
            }
        }

        public void CharacterTyped(char character)
        {
            byte[] data = encoding.GetBytes(new char[] { character });
            OnOutput(data);
        }

        public bool KeyPressed(int modifiers, int key)
        {
            return false;
        }

        public void Dispose()
        {
            commandBuffer.Clear();
        }

        protected abstract void OnCharacters(char[] characters);
        protected abstract void ProcessCommand(byte command, string parameter);

        protected virtual bool IsValidOneCharacterCommand(char command)
        {
            return false;
        }

        protected virtual bool IsValidParameterCharacter(char character)
        {
            return char.IsNumber(character)
                || character == ';'
                || character == '"'
                || character == '?';
        }

        protected virtual void OnOutput(byte[] output)
        {
            if (Output == null)
            {
                return;
            }

            if (supportXonXoff && xOffReceived)
            {
                outBuffer.Add(output);
            }
            else
            {
                Output(this, output);
            }
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

        protected void AddToCommandBuffer(byte data)
        {
            if (supportXonXoff)
            {
                if (data == XonCharacter || data == XoffCharacter)
                {
                    return;
                }
            }

            commandBuffer.Add(data);
        }

        protected void AddToCommandBuffer(byte[] data)
        {
            if (supportXonXoff)
            {
                commandBuffer.AddRange(
                    data.Where(b => b != XonCharacter && b != XoffCharacter));
            }
            else
            {
                commandBuffer.AddRange(data);
            }
        }

        protected void ProcessCommandBuffer()
        {
            state = DecoderState.Command;

            if (commandBuffer.Count > 1)
            {
                if (commandBuffer[0] != EscapeCharacter)
                {
                    throw new DecodeException("Internal error, first command character MUST be the escape character, please report this bug to the author.");
                }

                int start = 1;
                // Is this a one or two byte escape code?
                if (commandBuffer[start] == LeftBracketCharacter)
                {
                    start++;

                    // It is a two byte escape code, but we still need more data
                    if (commandBuffer.Count < 3)
                    {
                        return;
                    }
                }

                bool insideQuotes = false;
                int end = start;
                while (end < commandBuffer.Count && (IsValidParameterCharacter((char)commandBuffer[end]) || insideQuotes))
                {
                    if (commandBuffer[end] == '"')
                    {
                        insideQuotes = !insideQuotes;
                    }
                    end++;
                }

                if (commandBuffer.Count == 2 && IsValidOneCharacterCommand((char)commandBuffer[start]))
                {
                    end = commandBuffer.Count - 1;
                }
                if (end == commandBuffer.Count)
                {
                    // More data needed
                    return;
                }

                Decoder decoder = (this as IDecoder).Encoding.GetDecoder();
                byte[] parameterData = new byte[end - start];
                for (int i = 0; i < parameterData.Length; i++)
                {
                    parameterData[i] = commandBuffer[start + i];
                }
                int parameterLength = decoder.GetCharCount(parameterData, 0, parameterData.Length);
                char[] parameterChars = new char[parameterLength];
                decoder.GetChars(parameterData, 0, parameterData.Length, parameterChars, 0);
                string parameter = new string(parameterChars);

                byte command = commandBuffer[end];

                try
                {
                    ProcessCommand(command, parameter);
                }
                finally
                {
                    // Remove the processed commands
                    if (commandBuffer.Count == end - 1)
                    {
                        // All command bytes processed, we can go back to normal handling
                        commandBuffer.Clear();
                        state = DecoderState.Normal;
                    }
                    else
                    {
                        bool returnToNormalState = true;
                        for (int i = end + 1; i < commandBuffer.Count; i++)
                        {
                            if (commandBuffer[i] == EscapeCharacter)
                            {
                                commandBuffer.RemoveRange(0, i);
                                ProcessCommandBuffer();
                                returnToNormalState = false;
                            }
                            else
                            {
                                ProcessNormalInput(commandBuffer[i]);
                            }
                        }
                        if (returnToNormalState)
                        {
                            commandBuffer.Clear();

                            state = DecoderState.Normal;
                        }
                    }
                }
            }
        }

        protected void ProcessNormalInput(byte data)
        {
            if (data == EscapeCharacter)
            {
                throw new DecodeException("Internal error, ProcessNormalInput was passed an escape character, please report this bug to the author.");
            }

            if (supportXonXoff)
            {
                if (data == XonCharacter || data == XoffCharacter)
                {
                    return;
                }
            }

            byte[] inputData = new byte[] { data };
            int charCount = decoder.GetCharCount(inputData, 0, 1);
            char[] characters = new char[charCount];
            decoder.GetChars(inputData, 0, 1, characters, 0);

            if (charCount > 0)
            {
                OnCharacters(characters);
            }
        }
    }
}
