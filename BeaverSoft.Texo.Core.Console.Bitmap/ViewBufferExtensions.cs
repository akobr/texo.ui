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

            Size charSize = new Size((int)Math.Ceiling(font.Height / 2.0), font.Height);
            BitmapImage bitmap = new BitmapImage(charSize.Width * buffer.Screen.Width, charSize.Height * buffer.Screen.Height);
            Graphics graphics = Graphics.FromImage(bitmap);

            byte attributesId = 0;
            GraphicAttributes attributes = buffer.Styles[attributesId];
            Font usedFont = attributes.GetFont(font);

            for (int r = 0; r < buffer.Screen.Height; ++r)
            {
                for (int c = 0; c < buffer.Screen.Width; ++c)
                {
                    BufferCell character = buffer[c, r];
                    Rectangle rect = new Rectangle(charSize.Width * c, charSize.Height * r, charSize.Width, charSize.Height);
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
