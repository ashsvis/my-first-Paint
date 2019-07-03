using System;
using System.Drawing;

namespace PaintLibrary.Tools
{
    /// <summary>
    /// Style of pen
    /// </summary>
    [Serializable]
    public class PenStyle
    {
        public Color Color { get; set; } = Color.Black;
        public float Width { get; set; } = 1f;
        public float Opacity { get; set; } = 1f;
        public float Hardness { get; set; } = 1f;
    }
}