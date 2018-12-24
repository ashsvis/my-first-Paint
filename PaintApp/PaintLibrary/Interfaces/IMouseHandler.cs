using System.Drawing;
using PaintLibrary.Core;

namespace PaintLibrary.Tools
{
    /// <summary>
    /// The object handles mouse
    /// </summary>
    public interface IMouseHandler
    {
        void OnMouseDown(Document doc, Point point);
        void OnMouseMove(Document doc, Point point);
        void OnMouseUp(Document doc, Point point);
    }
}