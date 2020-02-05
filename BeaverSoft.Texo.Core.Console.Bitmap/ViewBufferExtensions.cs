using System;
using System.Drawing;
using BeaverSoft.Texo.Core.Console.Rendering;

using BitmapImage = System.Drawing.Bitmap;

namespace BeaverSoft.Texo.Core.Console.Bitmap
{
    public static class ViewBufferExtensions
    {
        public static BitmapImage ToBitmap(this ConsoleBuffer buffer)
        {
            return ToBitmap(buffer, new Font(new FontFamily("Consolas"), 12, FontStyle.Regular));
        }

        public static BitmapImage ToBitmap(this ConsoleBuffer buffer, Font font)
        {
            if (font == null)
            {
                throw new ArgumentNullException(nameof(font), "A font must be specified.");
            }

            BitmapImage bitmap = new BitmapImage(font.Height * buffer.Screen.Width, font.Height * buffer.Screen.Height);
            Graphics graphics = Graphics.FromImage(bitmap);

            byte attributesId = 0;
            GraphicAttributes attributes = buffer.Styles[attributesId];
            Font usedFont = attributes.GetFont(font);

            for (int y = 0; y < buffer.Screen.Height; ++y)
            {
                for (int x = 0; x < buffer.Screen.Width; ++x)
                {
                    BufferCell character = buffer[x, y];
                    Rectangle rect = new Rectangle(font.Height * x, font.Height * y, font.Height, font.Height);
                    graphics.FillRectangle(new SolidBrush(attributes.GetBackground()), rect);

                    if (character.StyleId != attributesId)
                    {
                        attributesId = character.StyleId;
                        attributes = buffer.Styles[attributesId];
                        usedFont = attributes.GetFont(font);
                    }

                    graphics.DrawString(character.Character.ToString(), usedFont, new SolidBrush(attributes.GetForeground()), rect);
                }
            }

            return bitmap;
        }
    }
}
