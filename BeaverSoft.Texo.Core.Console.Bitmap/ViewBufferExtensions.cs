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

            Size charSize = new Size((int)Math.Ceiling(font.Height / 2.0) + 1, font.Height);
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

                    if (character.StyleId != attributesId)
                    {
                        attributesId = character.StyleId;
                        attributes = buffer.Styles[attributesId];
                        usedFont = attributes.GetFont(font);
                    }

                    Rectangle rect = new Rectangle(charSize.Width * c, charSize.Height * r, charSize.Width, charSize.Height);
                    graphics.FillRectangle(new SolidBrush(attributes.GetBackground()), rect);
                    graphics.DrawString(character.Character.ToString(), usedFont, new SolidBrush(attributes.GetForeground()), rect);
                }
            }

            RenderCursor(buffer, charSize, graphics);
            RenderChangesOverlay(buffer, charSize, graphics);
            return bitmap;
        }

        private static void RenderChangesOverlay(ConsoleBuffer buffer, Size charSize, Graphics graphics)
        {
            int widthWithControl = buffer.Screen.Width + 1;

            foreach (Sequence change in buffer.Changes.Changes)
            {
                int rowStart = change.StartIndex / widthWithControl;
                int columnStart = change.StartIndex % widthWithControl;
                if (columnStart >= buffer.Screen.Width) columnStart = buffer.Screen.Width - 1;

                int rowEnd = change.EndIndex / widthWithControl;
                int columnEnd = change.EndIndex % widthWithControl;
                if (columnEnd >= buffer.Screen.Width) columnEnd = buffer.Screen.Width - 1;

                Rectangle cursorRect = new Rectangle(charSize.Width * columnStart, charSize.Height * rowStart, charSize.Width * (columnEnd - columnStart + 1), charSize.Height * (rowEnd - rowStart + 1));
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(42, 151, 242, 149)), cursorRect);
                graphics.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(142, 151, 242, 149)), 1), cursorRect);
            }
        }

        private static void RenderCursor(ConsoleBuffer buffer, Size charSize, Graphics graphics)
        {
            Rectangle cursorRect = new Rectangle(charSize.Width * buffer.Cursor.X, charSize.Height * buffer.Cursor.Y + charSize.Height - 5, charSize.Width, 5);
            graphics.FillRectangle(new SolidBrush(Color.White), cursorRect);
        }
    }
}
