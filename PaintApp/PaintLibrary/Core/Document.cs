using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintLibrary.Core
{
    /// <summary>
    /// Document
    /// </summary>
    public class Document
    {
        /// <summary>
        /// Current tool
        /// </summary>
        public Tool Tool { get; protected set; }

        /// <summary>
        /// Current layer
        /// </summary>
        public Bitmap Layer { get; set; }

        /// <summary>
        /// Render layer
        /// </summary>
        public void Render(Graphics gr)
        {
            if (Tool != null)
                Tool.Render(gr, this);
            else
                gr.DrawImage(Layer, Point.Empty);
        }
    }

}
