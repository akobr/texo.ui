using System.Collections.Generic;
using System.Drawing;

namespace BeaverSoft.Texo.View.Terminal
{
    public class TextStylesManager
    {
        public const int DEFAULT_STYLE_ID = 0;

        private readonly TextStyle[] styles;
        private readonly Dictionary<FontStyle, Dictionary<Color, Dictionary<Color, byte>>> styleMap;
        private byte styleId;

        public TextStylesManager()
        {
            styles = new TextStyle[byte.MaxValue + 1];
            styleMap = new Dictionary<FontStyle, Dictionary<Color, Dictionary<Color, byte>>>();
            InitialiseDefaultStyle();
        }

        public TextStyle this[byte styleId] => styles[styleId] ?? DefaultStyle;

        public TextStyle DefaultStyle => styles[DEFAULT_STYLE_ID];

        public byte GetOrCreateStyle(Color foreground, Color background, FontStyle fontStyles)
        {
            if (!styleMap.TryGetValue(fontStyles, out var backgroundMap))
            {
                styleMap[fontStyles] = backgroundMap = new Dictionary<Color, Dictionary<Color, byte>>();
            }

            if (!backgroundMap.TryGetValue(background, out var foregorundMap))
            {
                backgroundMap[background] = foregorundMap = new Dictionary<Color, byte>();
            }

            if (!foregorundMap.TryGetValue(foreground, out byte styleIndex))
            {
                TextStyle style = new TextStyle(new SolidBrush(foreground), new SolidBrush(background), fontStyles);
                styleIndex = GetNextStyleId();

                TryRemoveStyle(styleIndex);
                styles[styleIndex] = style;
                foregorundMap[foreground] = styleIndex;
            }

            return styleIndex;
        }

        private void InitialiseDefaultStyle()
        {
            var foregroundMap = new Dictionary<Color, byte>();
            foregroundMap[Color.Transparent] = DEFAULT_STYLE_ID;
            var backgroundMap = new Dictionary<Color, Dictionary<Color, byte>>();
            backgroundMap[Color.Transparent] = foregroundMap;
            styleMap[FontStyle.Regular] = backgroundMap;
            styles[DEFAULT_STYLE_ID] = new TextStyle(null, null, FontStyle.Regular);
        }

        private void TryRemoveStyle(byte index)
        {
            if (index >= styles.Length)
            {
                return;
            }

            TextStyle style = styles[index];

            styleMap
                [style.FontStyle]
                [((SolidBrush)style.BackgroundBrush)?.Color ?? Color.Transparent]
                .Remove(((SolidBrush)style.ForegroundBrush)?.Color ?? Color.Transparent);
        }

        private byte GetNextStyleId()
        {
            return ++styleId == DEFAULT_STYLE_ID ? ++styleId : styleId;
        }
    }
}
