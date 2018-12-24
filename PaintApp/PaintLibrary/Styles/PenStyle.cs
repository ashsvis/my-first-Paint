using System.Drawing;

namespace PaintLibrary.Tools
{
    /// <summary>
    /// Style of pen
    /// </summary>
    public class PenStyle
    {
        public Color Color { get; set; } = Color.Black;
        public float Width { get; set; } = 2;
        public float Hardness { get; set; } = 1;
    }
}