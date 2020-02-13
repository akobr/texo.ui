using System.Runtime.InteropServices;
using System.Text;

namespace BeaverSoft.Texo.Core.Console.Interop
{
    public static class Keyboard
    {
        private const int BUFFER_SIZE = 256;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int ToUnicode(
            uint virtualKeyCode,
            uint scanCode,
            byte[] keyboardState,
            StringBuilder receivingBuffer,
            int bufferSize,
            uint flags
        );

        public static string GetCharsFromKeys(Keys keys)
        {
            return GetCharsFromKeys((uint)keys);
        }

        public static string GetCharsFromKeys(uint keys)
        {
            StringBuilder buffer = new StringBuilder(BUFFER_SIZE);
            ToUnicode(keys, 0, new byte[BUFFER_SIZE], buffer, buffer.Capacity, 0);
            return buffer.ToString();
        }

        public static string GetCharsFromKeys(Keys keys, bool shift)
        {
            return GetCharsFromKeys((uint)keys, shift);
        }

        public static string GetCharsFromKeys(uint keys, bool shift)
        {
            StringBuilder buffer = new StringBuilder(BUFFER_SIZE);
            byte[] keyboardState = new byte[BUFFER_SIZE];

            if (shift)
            {
                keyboardState[(int)Keys.ShiftKey] = 0xff;
            }

            ToUnicode(keys, 0, keyboardState, buffer, BUFFER_SIZE, 0);
            return buffer.ToString();
        }
    }
}
