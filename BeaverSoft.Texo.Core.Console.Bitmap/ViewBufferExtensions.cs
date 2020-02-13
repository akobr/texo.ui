using System;
using System.Collections.Generic;
using System.Drawing;
using BeaverSoft.Texo.Core.Console.Rendering;

using BitmapImage = System.Drawing.Bitmap;

namespace BeaverSoft.Texo.Core.Console.Bitmap
{
    public static class ViewBufferExtensions
    {
        public static BitmapImage ToBitmap(this IConsoleBuffer buffer, bool showChangesOverlay = false)
        {
            return ToBitmap(buffer, new Font(new FontFamily("Consolas"), 12, FontStyle.Regular), showChangesOverlay);
        }

        public static BitmapImage ToBitmap(this IConsoleBuffer buffer, Font font, bool showChangesOverlay = false)
        {
            return ToBitmap(buffer, new Rectangle(0, 0, buffer.Screen.Width, buffer.Size.Height), font, showChangesOverlay);
        }

        public static BitmapImage ToScreenBitmap(this IConsoleBuffer buffer, bool showChangesOverlay = false)
        {
            return ToBitmap(buffer, new Font(new FontFamily("Consolas"), 12, FontStyle.Regular), showChangesOverlay);
        }

        public static BitmapImage ToScreenBitmap(this IConsoleBuffer buffer, Font font, bool showChangesOverlay = false)
        {
            return ToBitmap(buffer, buffer.Screen, font, showChangesOverlay);
        }

        public static BitmapImage ToBitmap(this IConsoleBuffer buffer, Rectangle areaToRender, Font font, bool showChangesOverlay = false)
        {
            if (font == null)
            {
                throw new ArgumentNullException(nameof(font), "A font must be specified.");
            }

            // TODO: [P3] Find better way how to calculate sizes
            Size charSize = new Size((int)Math.Ceiling(font.Height / 2.0) + 1, font.Height);
            BitmapImage bitmap = new BitmapImage(charSize.Width * areaToRender.Width, charSize.Height * areaToRender.Height);
            Graphics graphics = Graphics.FromImage(bitmap);

            byte attributesId = 0;
            GraphicAttributes attributes = buffer.Styles[attributesId];
            Font usedFont = attributes.GetFont(font);

            for (int r = areaToRender.Y; r < areaToRender.Height; ++r)
            {
                for (int c = areaToRender.X; c < areaToRender.Width; ++c)
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

            RenderCursor(buffer, buffer.Screen, charSize, graphics);

            if (showChangesOverlay)
            {
                RenderChangesOverlay(buffer, charSize, graphics);
            }

            return bitmap;
        }

        public static void RenderScreenChangesToExistingBitmap(this ConsoleBuffer buffer, BitmapImage target)
        {
            RenderScreenChangesToExistingBitmap(buffer, target, new Font(new FontFamily("Consolas"), 12, FontStyle.Regular));
        }

        public static void RenderScreenChangesToExistingBitmap(this ConsoleBuffer buffer, BitmapImage target, Font font)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target), "A target bitmap image must be specified.");
            }

            if (font == null)
            {
                throw new ArgumentNullException(nameof(font), "A font must be specified.");
            }

            // TODO: [P3] Find better way how to calculate sizes
            Size charSize = new Size((int)Math.Ceiling(font.Height / 2.0) + 1, font.Height);
            Graphics graphics = Graphics.FromImage(target);

            byte attributesId = 0;
            GraphicAttributes attributes = buffer.Styles[attributesId];
            Font usedFont = attributes.GetFont(font);

            int startScreenIndex = buffer.Screen.Y * buffer.Size.Width + buffer.Screen.X;
            int endScreenIndex = buffer.Screen.Bottom * buffer.Size.Width + buffer.Screen.Left;

            foreach (ConsoleBufferChange change in buffer.Changes.Changes)
            {
                int startIndex = change.Start.Y * buffer.Size.Width + change.Start.X;
                int endIndex = change.End.Y * buffer.Size.Width + change.End.X;

                for (int i = startIndex; i < endIndex; ++i)
                {
                    if (i < startScreenIndex
                        || i > endScreenIndex
                        || i % buffer.Size.Width >= buffer.Screen.Width)
                    {
                        continue;
                    }

                    BufferCell character = buffer[i];

                    if (character.StyleId != attributesId)
                    {
                        attributesId = character.StyleId;
                        attributes = buffer.Styles[attributesId];
                        usedFont = attributes.GetFont(font);
                    }

                    int r = i / buffer.Size.Width;
                    int c = i % buffer.Size.Width;

                    Rectangle rect = new Rectangle(charSize.Width * c, charSize.Height * r, charSize.Width, charSize.Height);
                    graphics.FillRectangle(new SolidBrush(attributes.GetBackground()), rect);
                    graphics.DrawString(character.Character.ToString(), usedFont, new SolidBrush(attributes.GetForeground()), rect);
                }
            }

            RenderNonCursor(buffer, buffer.Screen, charSize, graphics);
            RenderCursor(buffer, buffer.Screen, charSize, graphics);
        }

        private static void RenderChangesOverlay(IConsoleBuffer buffer, Size charSize, Graphics graphics)
        {
            foreach (ConsoleBufferChange change in buffer.Changes.Changes)
            {
                var polygon = BuildChangePolygon(change, buffer, charSize);
                graphics.FillPolygon(new SolidBrush(Color.FromArgb(100, 151, 242, 149)), polygon);
                graphics.DrawPolygon(new Pen(new SolidBrush(Color.FromArgb(200, 151, 242, 149)), 1), polygon);
            }
        }

        private static Point[] BuildChangePolygon(ConsoleBufferChange change, IConsoleBuffer buffer, Size charSize)
        {
            bool startIsOutsideRow = false;
            List<Point> polygon = new List<Point>();

            if (change.Start.X >= buffer.Screen.Width)
            {
                polygon.Add(new Point(0, charSize.Height * change.Start.Y + 1));
                startIsOutsideRow = true;
            }
            else
            {
                polygon.Add(new Point(charSize.Width * change.Start.X, charSize.Height * change.Start.Y));
            }

            if (change.Start.Y == change.End.Y) // One line
            {
                polygon.Add(new Point(charSize.Width * (change.End.X + 1), charSize.Height * change.End.Y));
                polygon.Add(new Point(charSize.Width * (change.End.X + 1), charSize.Height * (change.End.Y + 1)));
                polygon.Add(new Point(charSize.Width * change.Start.X, charSize.Height * (change.Start.Y + 1)));
                polygon.Add(polygon[0]);
                return polygon.ToArray();
            }
            else if (change.End.X >= buffer.Screen.Width - 1) // End is full row
            {
                polygon.Add(new Point((charSize.Width * buffer.Screen.Width) - 1, charSize.Height * change.Start.Y));
                polygon.Add(new Point((charSize.Width * buffer.Screen.Width) - 1, charSize.Height * (change.End.Y + 1)));
                polygon.Add(new Point(0, charSize.Height * (change.End.Y + 1)));
            }
            else // End isn't full row
            {
                polygon.Add(new Point((charSize.Width * buffer.Screen.Width) - 1, charSize.Height * change.Start.Y));
                polygon.Add(new Point((charSize.Width * buffer.Screen.Width) - 1, charSize.Height * change.End.Y));
                polygon.Add(new Point(charSize.Width * (change.End.X + 1), charSize.Height * change.End.Y));
                polygon.Add(new Point(charSize.Width * (change.End.X + 1), charSize.Height * (change.End.Y + 1)));
                polygon.Add(new Point(0, charSize.Height * (change.End.Y + 1)));
            }

            if (!startIsOutsideRow && change.Start.X != 0) // Start is full row
            {
                polygon.Add(new Point(0, charSize.Height * (change.Start.Y + 1)));
                polygon.Add(new Point(charSize.Width * change.Start.X, charSize.Height * (change.Start.Y + 1)));
            }

            polygon.Add(polygon[0]);
            return polygon.ToArray();
        }

        private static void RenderCursor(IConsoleBuffer buffer, Rectangle screen, Size charSize, Graphics graphics)
        {
            Rectangle cursorRect = new Rectangle(charSize.Width * (buffer.Cursor.X + screen.X), charSize.Height * (buffer.Cursor.Y + screen.Y) + charSize.Height - 5, charSize.Width, 5);
            graphics.FillRectangle(new SolidBrush(Color.White), cursorRect);
        }

        private static void RenderNonCursor(IConsoleBuffer buffer, Rectangle screen, Size charSize, Graphics graphics)
        {
            Rectangle cursorRect = new Rectangle(charSize.Width * (buffer.Changes.StartCursor.X + screen.X), charSize.Height * (buffer.Changes.StartCursor.Y + screen.Y) + charSize.Height - 5, charSize.Width, 5);
            graphics.FillRectangle(new SolidBrush(Color.Black), cursorRect);
        }
    }
}
