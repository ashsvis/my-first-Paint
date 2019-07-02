using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace PaintApp.Services
{
    public static class DrawUtils
    {
        static private List<Image> colorImages;

        static private List<NamedColor> namedColors = new List<NamedColor>()
        {
            new NamedColor(Color.Black, "Чёрный"),
            new NamedColor(Color.White, "Белый"),
            new NamedColor(Color.Red, "Красный"),
            new NamedColor(Color.Lime, "Ярко-зелёный"),
            new NamedColor(Color.Blue, "Синий"),
            new NamedColor(Color.Yellow, "Жёлтый"),
            new NamedColor(Color.Magenta, "Лиловый"),
            new NamedColor(Color.Cyan, "Бирюзовый"),
            new NamedColor(Color.Brown, "Коричневый"),
            new NamedColor(Color.Green, "Зелёный"),
            new NamedColor(Color.Navy, "Тёмно-синий"),
            new NamedColor(Color.Olive, "Коричнево-зелёный"),
            new NamedColor(Color.DarkMagenta, "Фиолетовый"),
            new NamedColor(Color.DarkCyan, "Тёмно-бирюзовый"),
            new NamedColor(Color.WhiteSmoke, "Серый 10%"),
            new NamedColor(Color.Gainsboro, "Серый 25%"),
            new NamedColor(Color.LightGray, "Серый 40%"),
            new NamedColor(Color.Silver, "Серый 50%"),
            new NamedColor(Color.DarkGray, "Серый 60%"),
            new NamedColor(Color.Gray, "Серый 75%"),
            new NamedColor(Color.DimGray, "Серый 90%"),
        };

        static private List<Color> customColors = new List<Color>();

        public static NamedColor[] GetAllColors()
        {
            int n = namedColors.Count + customColors.Count;
            NamedColor[] names = new NamedColor[n];
            int nc = 0;
            for (int i = 0; i < namedColors.Count; i++)
                names[nc++] = namedColors[i];
            for (int i = 0; i < customColors.Count; i++)
                names[nc++] = new NamedColor(customColors[i], String.Format(CultureInfo.InvariantCulture, "Цвет {0}", i));
            return names;
        }

        public static Image[] GetAllColorImages()
        {
            if (colorImages == null)
            {
                colorImages = new List<Image>();
                var rect = new Rectangle(0, 0, 15, 15);
                rect.Inflate(-1, -1);
                for (int i = 0; i < namedColors.Count; i++)
                {
                    var bmp = new Bitmap(16, 16);
                    using (var gr = Graphics.FromImage(bmp))
                    using (var brush = new SolidBrush(namedColors[i].Color))
                    {
                        gr.FillRectangle(brush, rect);
                        gr.DrawRectangle(Pens.Black, rect);
                    }
                    colorImages.Add(bmp);
                }
                for (int i = 0; i < customColors.Count; i++)
                {
                    var bmp = new Bitmap(16, 16);
                    using (var gr = Graphics.FromImage(bmp))
                    using (var brush = new SolidBrush(customColors[i]))
                    {
                        gr.FillRectangle(brush, rect);
                        gr.DrawRectangle(Pens.Black, rect);
                    }
                    colorImages.Add(bmp);
                }
            }
            return colorImages.ToArray();
        }

        public static bool FindColor(Color color)
        {
            bool found = false;
            for (int i = 0; i < namedColors.Count; i++) if (namedColors[i].Color == color) { found = true; break; }
            if (!found) { for (int i = 0; i < customColors.Count; i++) if (customColors[i] == color) { found = true; break; } }
            return found;
        }

        public static void AddCustomColor(Color color)
        {
            customColors.Add(color);
            var bmp = new Bitmap(16, 16);
            using (var gr = Graphics.FromImage(bmp))
            using (var brush = new SolidBrush(color))
            {
                var rect = new Rectangle(0, 0, 15, 15);
                rect.Inflate(-1, -1);
                gr.FillRectangle(brush, rect);
                gr.DrawRectangle(Pens.Black, rect);
            }
            if (colorImages == null)
                colorImages = new List<Image>();
            colorImages.Add(bmp);
        }

        public static int[] GetCustomColors()
        {
            int count = customColors.Count;
            int[] argbColors = new int[count];
            for (int i = 0; i < count; i++) argbColors[i] = customColors[i].ToArgb();
            return argbColors;
        }

    }
}
