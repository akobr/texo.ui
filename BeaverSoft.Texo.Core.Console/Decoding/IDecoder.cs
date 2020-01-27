using System;
using System.Text;

namespace BeaverSoft.Texo.Core.Console.Decoding
{
    public interface IDecoder : IDisposable
    {
        Encoding Encoding { get; }

        void Input(byte[] data);

        bool KeyPressed(int modifiers, int key);

        void CharacterTyped(char character);
    }
}
