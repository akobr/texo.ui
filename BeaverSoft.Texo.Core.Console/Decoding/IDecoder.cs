using System;
using System.Text;

namespace BeaverSoft.Texo.Core.Console.Decoding
{
    public interface IDecoder : IDisposable
    {
        Encoding Encoding { get; }

        void Input(byte[] data);

        bool KeyPressed(Keys modifiers, Keys key);

        void CharacterTyped(char character);
    }
}
