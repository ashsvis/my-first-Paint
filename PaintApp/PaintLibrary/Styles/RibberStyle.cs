using System;
using System.Drawing;

namespace PaintLibrary.Tools
{
    /// <summary>
    /// Style of ribber
    /// </summary>
    [Serializable]
    public class RibberStyle
    {
        public Color Color { get; set; } = Color.FromArgb(254, 1, 254); //Color.Magenta;
        public float Width { get; set; } = 4f;
        public float Opacity { get; set; } = 1f;
        public float Hardness { get; set; } = 1f;
    }
}