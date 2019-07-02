using System.Drawing;

namespace PaintApp.Services
{
    public class NamedColor
    {
        public NamedColor(Color color, string colorName)
        {
            this.Color = color;
            this.Name = colorName;
        }
        public Color Color { get; set; }
        public string Name { get; set; }
    }
}
