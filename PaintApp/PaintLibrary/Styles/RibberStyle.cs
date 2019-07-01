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
        public Color Color { get; set; } = Color.White;
        public float Width { get; set; } = 5;
    }
}